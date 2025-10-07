using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.FontManagement
{
    public static class FontListHelper
    {
        // This tracks all discovered fonts (system, local or embedded)
        public static List<FontConfiguration> FontConfigurations { get; set; } = new List<FontConfiguration>();

        // A collection to manage private fonts
        private static PrivateFontCollection privateFontCollection = new PrivateFontCollection();

        // A simple incremental index
        private static int index = -1;

        // Cache created fonts to reduce allocations - but track if they're still valid
        private static readonly Dictionary<string, WeakReference<Font>> fontCache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly object fontCacheLock = new object();

        #region "System Font Discovery"
        /// <summary>
        /// Gets all installed system fonts and adds them to FontConfigurations
        /// </summary>
        /// <returns>A list of newly discovered FontConfiguration objects for system fonts</returns>
        public static List<FontConfiguration> GetSystemFonts()
        {
            var result = new List<FontConfiguration>();

            using (var fontCollection = new InstalledFontCollection())
            {
                foreach (var fontFamily in fontCollection.Families)
                {
                    // Check if this font is already in our list
                    bool alreadyInList = FontConfigurations.Any(
                        cfg => cfg.Name.Equals(fontFamily.Name, StringComparison.OrdinalIgnoreCase));

                    if (!alreadyInList)
                    {
                        var config = new FontConfiguration
                        {
                            Index = ++index,
                            Name = fontFamily.Name,
                            IsSystemFont = true,
                            IsPrivateFont = false,
                            IsEmbeddedResource = false,
                            StylesAvailable = GetAvailableStyles(fontFamily)
                        };

                        result.Add(config);
                    }
                }
            }

            // Add newly discovered fonts to the global list
            if (result.Count > 0)
            {
                FontConfigurations.AddRange(result);
            }

            return result;
        }

        private static List<FontStyle> GetAvailableStyles(FontFamily fontFamily)
        {
            var styles = new List<FontStyle>();

            if (fontFamily.IsStyleAvailable(FontStyle.Regular))
                styles.Add(FontStyle.Regular);
            if (fontFamily.IsStyleAvailable(FontStyle.Bold))
                styles.Add(FontStyle.Bold);
            if (fontFamily.IsStyleAvailable(FontStyle.Italic))
                styles.Add(FontStyle.Italic);
            if (fontFamily.IsStyleAvailable(FontStyle.Bold | FontStyle.Italic))
                styles.Add(FontStyle.Bold | FontStyle.Italic);
            if (fontFamily.IsStyleAvailable(FontStyle.Underline))
                styles.Add(FontStyle.Underline);
            if (fontFamily.IsStyleAvailable(FontStyle.Strikeout))
                styles.Add(FontStyle.Strikeout);

            return styles;
        }
        #endregion

        #region "Local Font File Discovery"
        /// <summary>
        /// Scans the given folder for font files (.ttf, .otf), creates FontConfiguration for each,
        /// adds them to FontConfigurations, and loads them into the privateFontCollection.
        /// </summary>
        /// <param name="path">The folder to scan for font files</param>
        /// <returns>A list of newly discovered FontConfiguration objects</returns>
        public static List<FontConfiguration> GetFontFilesLocations(string path)
        {
            var result = new List<FontConfiguration>();
            if (string.IsNullOrEmpty(path))
            {
                return result;
            }
            if (!Directory.Exists(path))
            {
                return result;
            }

            // Get all font files in the directory and subdirectories
            string[] fontFiles = Directory.GetFiles(path, "*.ttf", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(path, "*.otf", SearchOption.AllDirectories))
                .Concat(Directory.GetFiles(path, "*.ttc", SearchOption.AllDirectories))
                .ToArray();

            foreach (string file in fontFiles)
            {
                string filename = Path.GetFileName(file);

                // Check if already in FontConfigurations
                bool alreadyInList = FontConfigurations.Any(
                    cfg => string.Equals(cfg.Path, file, StringComparison.OrdinalIgnoreCase));

                if (!alreadyInList)
                {
                    try
                    {
                        // Add font to private font collection
                        privateFontCollection.AddFontFile(file);

                        // Try to resolve family by reading the last added family or by name
                        int lastIndex = privateFontCollection.Families.Length - 1;
                        FontFamily fontFamily = privateFontCollection.Families[lastIndex];

                        var config = new FontConfiguration
                        {
                            Index = ++index,
                            Name = fontFamily?.Name ?? Path.GetFileNameWithoutExtension(file),
                            Path = file,
                            FileName = filename,
                            IsSystemFont = false,
                            IsPrivateFont = true,
                            IsEmbeddedResource = false,
                            PrivateFontIndex = lastIndex,
                            StylesAvailable = fontFamily != null ? GetAvailableStyles(fontFamily) : new List<FontStyle>()
                        };

                        result.Add(config);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to load font file {file}: {ex.Message}");
                    }
                }
            }

            // Add newly discovered fonts to the global list
            if (result.Count > 0)
            {
                FontConfigurations.AddRange(result);
            }

            return result;
        }
        #endregion

        #region "Embedded Resource Font Discovery"
        public sealed class EmbeddedScanOptions
        {
            public string[] Namespaces { get; set; } = Array.Empty<string>();
            public bool IncludeFrameworkAssemblies { get; set; } = false;
            public bool IncludeReferencedAssemblies { get; set; } = true;
            public int MaxReferenceDepth { get; set; } = 2;
        }

        /// <summary>
        /// Scans assemblies for embedded font resources (.ttf, .otf),
        /// creates FontConfiguration for each, and adds them to FontConfigurations.
        /// </summary>
        /// <param name="namespaces">Optional array of namespace strings to filter resources</param>
        /// <returns>A list of newly discovered FontConfiguration objects</returns>
        public static List<FontConfiguration> GetFontResourcesFromEmbedded(string[] namespaces = null)
        {
            var options = new EmbeddedScanOptions { Namespaces = namespaces ?? Array.Empty<string>() };
            return GetFontResourcesFromEmbedded(options);
        }

        public static List<FontConfiguration> GetFontResourcesFromEmbedded(EmbeddedScanOptions options)
        {
            var result = new List<FontConfiguration>();
            options ??= new EmbeddedScanOptions();

            var assemblySet = new HashSet<Assembly>();

            void TryAddAssembly(Assembly asm)
            {
                if (asm == null) return;
                if (asm.IsDynamic) return;
                if (!options.IncludeFrameworkAssemblies)
                {
                    var fn = asm.FullName ?? string.Empty;
                    if (fn.StartsWith("System", StringComparison.OrdinalIgnoreCase) ||
                        fn.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase))
                        return;
                }
                assemblySet.Add(asm);
            }

            TryAddAssembly(Assembly.GetExecutingAssembly());
            TryAddAssembly(Assembly.GetCallingAssembly());
            TryAddAssembly(Assembly.GetEntryAssembly());

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                TryAddAssembly(asm);
            }

            if (options.IncludeReferencedAssemblies && options.MaxReferenceDepth > 0)
            {
                int depth = 0;
                var queue = new Queue<(Assembly asm, int depth)>();
                foreach (var a in assemblySet) queue.Enqueue((a, 0));
                while (queue.Count > 0)
                {
                    var (asm, d) = queue.Dequeue();
                    if (d >= options.MaxReferenceDepth) continue;
                    AssemblyName[] refs;
                    try { refs = asm.GetReferencedAssemblies(); }
                    catch { continue; }
                    foreach (var an in refs)
                    {
                        try
                        {
                            if (!options.IncludeFrameworkAssemblies)
                            {
                                if (an.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase) ||
                                    an.FullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase))
                                    continue;
                            }
                            var loaded = Assembly.Load(an);
                            if (loaded != null && !loaded.IsDynamic && assemblySet.Add(loaded))
                            {
                                queue.Enqueue((loaded, d + 1));
                            }
                        }
                        catch { /* ignore */ }
                    }
                }
            }

            foreach (Assembly assembly in assemblySet)
            {
                string[] resources;
                try { resources = assembly.GetManifestResourceNames(); }
                catch { continue; }

                foreach (string resource in resources)
                {
                    // Check if resource matches namespace filter if provided
                    if (options.Namespaces != null && options.Namespaces.Length > 0)
                    {
                        if (!options.Namespaces.Any(ns => resource.IndexOf(ns, StringComparison.OrdinalIgnoreCase) >= 0))
                            continue;
                    }

                    string extension = Path.GetExtension(resource)?.ToLowerInvariant();
                    if (extension != ".ttf" && extension != ".otf" && extension != ".ttc" &&
                        extension != ".woff" && extension != ".woff2")
                        continue;

                    // Check if already in our list
                    bool alreadyInList = FontConfigurations.Any(
                        cfg => string.Equals(cfg.Path, resource, StringComparison.OrdinalIgnoreCase)
                               && string.Equals(cfg.AssemblyFullName, assembly.FullName, StringComparison.OrdinalIgnoreCase));

                    if (alreadyInList) continue;

                    try
                    {
                        using (Stream fontStream = assembly.GetManifestResourceStream(resource))
                        {
                            if (fontStream != null)
                            {
                                // Create a byte array of the right size
                                byte[] fontData = new byte[fontStream.Length];
                                fontStream.Read(fontData, 0, (int)fontStream.Length);

                                // Add font data to private font collection
                                IntPtr dataPointer = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
                                System.Runtime.InteropServices.Marshal.Copy(fontData, 0, dataPointer, fontData.Length);
                                privateFontCollection.AddMemoryFont(dataPointer, fontData.Length);
                                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(dataPointer);

                                // Get the index of the last added font
                                int lastIndex = privateFontCollection.Families.Length - 1;
                                var family = privateFontCollection.Families[lastIndex];
                                string fontName = family?.Name ?? Path.GetFileNameWithoutExtension(resource);

                                var config = new FontConfiguration
                                {
                                    Index = ++index,
                                    Name = fontName,
                                    Path = resource,
                                    FileName = Path.GetFileName(resource),
                                    IsSystemFont = false,
                                    IsPrivateFont = true,
                                    IsEmbeddedResource = true,
                                    AssemblyFullName = assembly.FullName,
                                    AssemblyLocation = SafeGetAssemblyLocation(assembly),
                                    PrivateFontIndex = lastIndex,
                                    StylesAvailable = family != null ? GetAvailableStyles(family) : new List<FontStyle>()
                                };

                                result.Add(config);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to load embedded font resource {resource}: {ex.Message}");
                    }
                }
            }

            // Add newly discovered fonts to the global list
            if (result.Count > 0)
            {
                FontConfigurations.AddRange(result);
            }

            return result;
        }

        private static string SafeGetAssemblyLocation(Assembly asm)
        {
            try { return asm?.Location; } catch { return null; }
        }
        #endregion

        #region "Loader Orchestration"
        public sealed class FontScanOptions
        {
            public List<string> Directories { get; set; } = new();
            public string[] EmbeddedNamespaces { get; set; } = new[]
            {
                "TheTechIdea.Beep.Winform.Controls.Fonts",
                "TheTechIdea.Beep.Fonts"
            };
            public bool IncludeFrameworkAssemblies { get; set; } = false;
            public bool IncludeReferencedAssemblies { get; set; } = true;
            public int MaxReferenceDepth { get; set; } = 2;
            public bool ScanSystemFonts { get; set; } = true;
        }

        /// <summary>
        /// Load fonts from system, local folders and embedded resources according to options.
        /// </summary>
        public static void LoadAllFonts(FontScanOptions options = null)
        {
            options ??= new FontScanOptions();

            if (options.ScanSystemFonts)
                GetSystemFonts();

            // folders
            foreach (var dir in options.Directories.Distinct().Where(d => !string.IsNullOrWhiteSpace(d)))
            {
                try { GetFontFilesLocations(dir); } catch { /* ignore */ }
            }

            // embedded
            var emb = new EmbeddedScanOptions
            {
                Namespaces = options.EmbeddedNamespaces ?? Array.Empty<string>(),
                IncludeFrameworkAssemblies = options.IncludeFrameworkAssemblies,
                IncludeReferencedAssemblies = options.IncludeReferencedAssemblies,
                MaxReferenceDepth = options.MaxReferenceDepth
            };
            GetFontResourcesFromEmbedded(emb);
        }
        #endregion

        #region "Font Access Methods"
        /// <summary>
        /// Creates a Font from a TypographyStyle object with null handling
        /// </summary>
        /// <param name="style">The TypographyStyle to convert to a Font</param>
        /// <returns>A Font object created from the style, or a default font if style is null</returns>
        public static Font CreateFontFromTypography(TypographyStyle style)
        {
            if (style == null)
            {
                // Return default font if style is null
                return GetFontWithFallback("Arial", "Segoe UI", 9.0f, FontStyle.Regular);
            }

            // Start with basic font style
            FontStyle fontStyle = style.FontStyle;

            // Add underline if specified
            if (style.IsUnderlined)
                fontStyle |= FontStyle.Underline;

            // Add strikeout if specified
            if (style.IsStrikeout)
                fontStyle |= FontStyle.Strikeout;

            // Convert FontWeight enum to FontStyle if needed
            if (style.FontWeight >= FontWeight.Bold)
                fontStyle |= FontStyle.Bold;

            // Get the font with fallback
            return GetFontWithFallback(
                style.FontFamily,
                "Arial",
                style.FontSize,
                fontStyle
            );
        }

        /// <summary>
        /// Gets a font by name with specified size and style. Uses cache and robust matching.
        /// </summary>
        public static Font GetFont(string fontName, float size, FontStyle style = FontStyle.Regular)
        {
            if (string.IsNullOrWhiteSpace(fontName))
            {
                return GetOrCreateFont("Arial|" + size + "|" + (int)style, 
                    () => new Font("Arial", size, style));
            }

            string cacheKey = $"{fontName}|{size}|{(int)style}";
            
            return GetOrCreateFont(cacheKey, () =>
            {
                // Try exact configuration match
                var fontConfig = FontConfigurations.FirstOrDefault(
                    f => f.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));

                // For system fonts, create from name
                if (fontConfig != null && fontConfig.IsSystemFont)
                {
                    return new Font(fontName, size, style);
                }
                
                // For private fonts, try by stored index, else by name lookup in private collection
                if (fontConfig != null && fontConfig.IsPrivateFont)
                {
                    if (fontConfig.PrivateFontIndex >= 0 && fontConfig.PrivateFontIndex < privateFontCollection.Families.Length)
                    {
                        var fam = privateFontCollection.Families[fontConfig.PrivateFontIndex];
                        if (fam != null)
                        {
                            return new Font(fam, size, style);
                        }
                    }

                    // fallback: search by name in private collection
                    var pfam = FindPrivateFamilyByName(fontName);
                    if (pfam != null)
                    {
                        return new Font(pfam, size, style);
                    }
                }

                // As a robust fallback: if some other config has close name match
                var alt = FontConfigurations.FirstOrDefault(f => NormalizeFontName(f.Name) == NormalizeFontName(fontName));
                if (alt != null)
                {
                    // system or private
                    if (alt.IsSystemFont)
                    {
                        return new Font(alt.Name, size, style);
                    }
                    var pfam2 = FindPrivateFamilyByName(alt.Name);
                    if (pfam2 != null)
                    {
                        return new Font(pfam2, size, style);
                    }
                }

                // Fallback: Try Arial, then GenericSansSerif
                try
                {
                    return new Font("Arial", size, style);
                }
                catch
                {
                    return new Font(FontFamily.GenericSansSerif, size, style);
                }
            });
        }

        private static string NormalizeFontName(string name)
        {
            return (name ?? string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty)
                .Replace(" ", string.Empty)
                .Trim()
                .ToLowerInvariant();
        }

        private static FontFamily FindPrivateFamilyByName(string name)
        {
            var target = NormalizeFontName(name);
            foreach (var fam in privateFontCollection.Families)
            {
                if (NormalizeFontName(fam.Name) == target)
                    return fam;
            }
            return null;
        }

        /// <summary>
        /// Gets a font by index with specified size and style
        /// </summary>
        public static Font GetFontByIndex(int index, float size, FontStyle style = FontStyle.Regular)
        {
            if (index < 0 || index >= FontConfigurations.Count)
                return null;

            var fontConfig = FontConfigurations[index];
            return GetFont(fontConfig.Name, size, style);
        }

        /// <summary>
        /// Gets the index of a font by name
        /// </summary>
        public static int GetFontIndex(string fontName)
        {
            if (string.IsNullOrEmpty(fontName))
                return -1;

            var fontConfig = FontConfigurations.FirstOrDefault(
                f => f.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));

            return fontConfig?.Index ?? -1;
        }

        /// <summary>
        /// Gets a list of all available font names
        /// </summary>
        public static List<string> GetFontNames()
        {
            return FontConfigurations.Select(f => f.Name).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        /// <summary>
        /// Gets a list of system font names
        /// </summary>
        public static List<string> GetSystemFontNames()
        {
            return FontConfigurations.Where(f => f.IsSystemFont).Select(f => f.Name).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        /// <summary>
        /// Gets a list of private font names
        /// </summary>
        public static List<string> GetPrivateFontNames()
        {
            return FontConfigurations.Where(f => f.IsPrivateFont).Select(f => f.Name).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        /// <summary>
        /// Gets a font with a fallback option if the primary font is not available
        /// </summary>
        public static Font GetFontWithFallback(string primaryFontName, string fallbackFontName, float size, FontStyle style = FontStyle.Regular)
        {
            // Try to get the primary font
            Font primaryFont = GetFont(primaryFontName, size, style);
            if (primaryFont != null)
                return primaryFont;

            // If primary font not found, try the fallback font
            Font fallbackFont = GetFont(fallbackFontName, size, style);
            if (fallbackFont != null)
                return fallbackFont;

            // If fallback also fails, use generic sans serif
            try
            {
                return new Font(FontFamily.GenericSansSerif, size, style);
            }
            catch
            {
                // Last resort: try with Arial which is commonly available
                try
                {
                    return new Font("Arial", size, style);
                }
                catch
                {
                    // If all else fails, return null
                    return null;
                }
            }
        }

        /// <summary>
        /// Safely gets a font from cache or creates a new one. Never returns null or disposed fonts.
        /// </summary>
        private static Font GetOrCreateFont(string cacheKey, Func<Font> fontFactory)
        {
            lock (fontCacheLock)
            {
                // Try to get from cache
                if (fontCache.TryGetValue(cacheKey, out var weakRef))
                {
                    if (weakRef.TryGetTarget(out var cachedFont))
                    {
                        // Validate the font is still usable
                        try
                        {
                            // Test if font is valid by accessing a property
                            var _ = cachedFont.Size;
                            return cachedFont;
                        }
                        catch
                        {
                            // Font is disposed or invalid, remove from cache
                            fontCache.Remove(cacheKey);
                        }
                    }
                    else
                    {
                        // WeakReference lost the font, remove from cache
                        fontCache.Remove(cacheKey);
                    }
                }

                // Create new font
                try
                {
                    Font newFont = fontFactory();
                    if (newFont != null)
                    {
                        // Validate new font before caching
                        try
                        {
                            var _ = newFont.Size; // Test if valid
                            fontCache[cacheKey] = new WeakReference<Font>(newFont);
                            return newFont;
                        }
                        catch
                        {
                            newFont?.Dispose();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Font creation failed: {ex.Message}");
                }

                // Ultimate fallback - always return a valid font
                return GetUltimateFallbackFont();
            }
        }

        /// <summary>
        /// Returns a guaranteed valid font as last resort
        /// </summary>
        private static Font GetUltimateFallbackFont()
        {
            try
            {
                return new Font(FontFamily.GenericSansSerif, 9f, FontStyle.Regular);
            }
            catch
            {
                try
                {
                    return SystemFonts.DefaultFont;
                }
                catch
                {
                    // Absolute last resort - this should never fail
                    return new Font("Arial", 9f);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Configuration class for storing font metadata
    /// </summary>
    public class FontConfiguration
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public bool IsSystemFont { get; set; }
        public bool IsPrivateFont { get; set; }
        public bool IsEmbeddedResource { get; set; }
        public string AssemblyFullName { get; set; }
        public string AssemblyLocation { get; set; }
        public int PrivateFontIndex { get; set; } = -1;
        public List<FontStyle> StylesAvailable { get; set; } = new List<FontStyle>();
    }
}
