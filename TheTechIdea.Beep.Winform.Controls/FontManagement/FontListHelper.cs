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
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

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

        // Tracks whether fonts were initialized to allow safe app-start calls
        private static bool _fontsLoaded = false;
        private static readonly object _initLock = new object();

        #region DPI Scaling Support

        // Current DPI scale factor (1.0 = 96 DPI)
        private static float _dpiScaleFactor = 1.0f;

        /// <summary>
        /// Gets or sets whether DPI scaling should be applied to fonts automatically.
        /// Default is false for backward compatibility. Set to true to enable DPI-aware font scaling.
        /// </summary>
        public static bool EnableDpiFontScaling { get; set; } = false;

        /// <summary>
        /// Gets or sets the current DPI scale factor used for font scaling.
        /// Default is 1.0 (96 DPI). Set this when DPI changes or at application startup.
        /// </summary>
        public static float DpiScaleFactor
        {
            get => _dpiScaleFactor;
            set
            {
                if (value > 0 && Math.Abs(_dpiScaleFactor - value) > 0.01f)
                {
                    _dpiScaleFactor = value;
                    // Clear cache when DPI changes since cached fonts have old sizes
                    ClearFontCache();
                }
            }
        }

        /// <summary>
        /// Updates the DPI scale factor from a Control's current DPI.
        /// </summary>
        public static void UpdateDpiFromControl(Control control)
        {
            if (control == null) return;
            DpiScaleFactor = DpiScalingHelper.GetDpiScaleFactor(control);
        }

        /// <summary>
        /// Updates the DPI scale factor using the system-wide DPI.
        /// </summary>
        public static void UpdateDpiFromSystem()
        {
            DpiScaleFactor = DpiScalingHelper.GetSystemDpiScaleFactor();
        }

        /// <summary>
        /// Applies DPI scaling to a font size if DPI scaling is enabled.
        /// </summary>
        private static float ApplyDpiScaling(float size)
        {
            if (!EnableDpiFontScaling || Math.Abs(_dpiScaleFactor - 1.0f) < 0.01f)
                return size;

            float scaledSize = size * _dpiScaleFactor;
            // Ensure minimum readable font size
            return Math.Max(scaledSize, 6.0f);
        }

        /// <summary>
        /// Applies DPI scaling to a font size using a specific scale factor.
        /// </summary>
        private static float ApplyDpiScaling(float size, float dpiScaleFactor)
        {
            if (Math.Abs(dpiScaleFactor - 1.0f) < 0.01f)
                return size;

            float scaledSize = size * dpiScaleFactor;
            return Math.Max(scaledSize, 6.0f);
        }

        #endregion

        /// <summary>
        /// Ensures scanning is performed once even if callers forget to call EnsureFontsLoaded.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (_fontsLoaded) return;
            lock (_initLock)
            {
                if (_fontsLoaded) return;
                try { EnsureFontsLoaded(null, false); } catch { }
            }
        }

        // Strong font cache - fonts live for app lifetime to avoid allocation spam in painters
        private static readonly Dictionary<string, Font> fontCache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly object fontCacheLock = new object();

        /// <summary>
        /// Gets a cached font for common painter scenarios (avoids per-paint allocation)
        /// </summary>
        public static Font GetCachedFont(string fontName, float size, FontStyle style = FontStyle.Regular)
        {
            // Apply DPI scaling if enabled
            float scaledSize = ApplyDpiScaling(size);
            string key = $"{fontName}|{scaledSize}|{(int)style}";
            lock (fontCacheLock)
            {
                if (fontCache.TryGetValue(key, out var cached))
                    return cached;
                
                var font = GetFontInternal(fontName, scaledSize, style);
                if (font != null)
                    fontCache[key] = font;
                return font;
            }
        }

        /// <summary>
        /// Gets a cached font with explicit DPI scale factor.
        /// </summary>
        public static Font GetCachedFont(string fontName, float size, float dpiScaleFactor, FontStyle style = FontStyle.Regular)
        {
            float scaledSize = ApplyDpiScaling(size, dpiScaleFactor);
            string key = $"{fontName}|{scaledSize}|{(int)style}";
            lock (fontCacheLock)
            {
                if (fontCache.TryGetValue(key, out var cached))
                    return cached;
                
                var font = GetFontInternal(fontName, scaledSize, style);
                if (font != null)
                    fontCache[key] = font;
                return font;
            }
        }

        /// <summary>
        /// Gets a cached font scaled for a specific control's DPI.
        /// </summary>
        public static Font GetCachedFontForControl(string fontName, float size, Control control, FontStyle style = FontStyle.Regular)
        {
            float dpiScale = control != null ? DpiScalingHelper.GetDpiScaleFactor(control) : 1.0f;
            return GetCachedFont(fontName, size, dpiScale, style);
        }

        /// <summary>
        /// Clears the font cache (use sparingly - only on theme changes or cleanup)
        /// </summary>
        public static void ClearFontCache()
        {
            lock (fontCacheLock)
            {
                fontCache.Clear();
            }
        }

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

            // BeepFontPaths-driven embedded fonts (explicit, reliable enumeration)
            try { LoadFontsFromBeepFontPaths(); } catch { /* ignore */ }
        }

        /// <summary>
        /// Ensures fonts are loaded once. Call this at app startup.
        /// Pass forceReload=true to rescan sources.
        /// </summary>
        public static void EnsureFontsLoaded(FontScanOptions options = null, bool forceReload = false)
        {
            if (!forceReload && _fontsLoaded)
                return;

            LoadAllFonts(options);
            _fontsLoaded = true;
        }
        #endregion

        #region "Font Access Methods"
        /// <summary>
        /// Loads embedded fonts declared in BeepFontPaths into the private collection and FontConfigurations.
        /// Ensures all Beep-provided fonts are available for GetFont lookups.
        /// </summary>
        private static void LoadFontsFromBeepFontPaths()
        {
            try
            {
                var resources = BeepFontPaths.GetAvailableFontResources();
                if (resources == null || resources.Length == 0)
                    return;

                foreach (var res in resources)
                {
                    // Skip if already added (by path+assembly match)
                    bool alreadyInList = FontConfigurations.Any(
                        cfg => string.Equals(cfg.Path, res, StringComparison.OrdinalIgnoreCase)
                               && string.Equals(cfg.AssemblyFullName, BeepFontPaths.ResourceAssembly.FullName, StringComparison.OrdinalIgnoreCase));
                    if (alreadyInList)
                        continue;

                    using (var s = BeepFontPaths.ResourceAssembly.GetManifestResourceStream(res))
                    {
                        if (s == null)
                            continue;
                        var fontData = new byte[s.Length];
                        s.Read(fontData, 0, fontData.Length);
                        var ptr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
                        try
                        {
                            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, ptr, fontData.Length);
                            privateFontCollection.AddMemoryFont(ptr, fontData.Length);
                        }
                        finally
                        {
                            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(ptr);
                        }

                        // Use the last-added family for metadata
                        int lastIndex = privateFontCollection.Families.Length - 1;
                        var family = lastIndex >= 0 ? privateFontCollection.Families[lastIndex] : null;
                        string fontName = family?.Name ?? BeepFontPaths.ExtractFontNameFromPath(res);

                        var config = new FontConfiguration
                        {
                            Index = ++index,
                            Name = fontName,
                            Path = res,
                            FileName = System.IO.Path.GetFileName(res),
                            IsSystemFont = false,
                            IsPrivateFont = true,
                            IsEmbeddedResource = true,
                            AssemblyFullName = BeepFontPaths.ResourceAssembly.FullName,
                            AssemblyLocation = SafeGetAssemblyLocation(BeepFontPaths.ResourceAssembly),
                            PrivateFontIndex = lastIndex,
                            StylesAvailable = family != null ? GetAvailableStyles(family) : new List<FontStyle>()
                        };

                        FontConfigurations.Add(config);
                    }
                }
            }
            catch
            {
                // ignore and continue; other loaders cover most cases
            }
        }
        /// <summary>
        /// Creates a Font from a TypographyStyle object with null handling.
        /// Applies DPI scaling if EnableDpiFontScaling is true.
        /// </summary>
        /// <param name="style">The TypographyStyle to convert to a Font</param>
        /// <returns>A Font object created from the Style, or a default font if Style is null</returns>
        public static Font CreateFontFromTypography(TypographyStyle style)
        {
            return CreateFontFromTypography(style, EnableDpiFontScaling);
        }

        /// <summary>
        /// Creates a Font from a TypographyStyle object with explicit DPI scaling control.
        /// </summary>
        public static Font CreateFontFromTypography(TypographyStyle style, bool applyDpiScaling)
        {
            EnsureInitialized();
            if (style == null)
            {
                // Return default font if Style is null
                return GetFont("Segoe UI", 9.0f, FontStyle.Regular);
            }

            // Start with basic font Style
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

            float fontSize = style.FontSize;
            if (applyDpiScaling)
            {
                fontSize = ApplyDpiScaling(fontSize);
            }

            // Get the font directly - GetFontInternal handles all fallbacks internally
            return GetFontInternal(
                style.FontFamily,
                fontSize,
                fontStyle
            );
        }

        /// <summary>
        /// Creates a Font from a TypographyStyle scaled for a specific control's DPI.
        /// </summary>
        public static Font CreateFontFromTypographyForControl(TypographyStyle style, Control control)
        {
            EnsureInitialized();
            if (style == null)
            {
                return GetFont("Segoe UI", 9.0f, FontStyle.Regular);
            }

            float dpiScale = control != null ? DpiScalingHelper.GetDpiScaleFactor(control) : 1.0f;

            FontStyle fontStyle = style.FontStyle;
            if (style.IsUnderlined)
                fontStyle |= FontStyle.Underline;
            if (style.IsStrikeout)
                fontStyle |= FontStyle.Strikeout;
            if (style.FontWeight >= FontWeight.Bold)
                fontStyle |= FontStyle.Bold;

            float fontSize = ApplyDpiScaling(style.FontSize, dpiScale);

            return GetFontInternal(style.FontFamily, fontSize, fontStyle);
        }

        /// <summary>
        /// Gets a font by name with specified size and Style. Uses cache and robust matching.
        /// Applies DPI scaling if EnableDpiFontScaling is true.
        /// </summary>
        public static Font GetFont(string fontName, float size, FontStyle style = FontStyle.Regular)
        {
            // Apply DPI scaling if enabled
            float scaledSize = ApplyDpiScaling(size);
            return GetFontInternal(fontName, scaledSize, style);
        }

        /// <summary>
        /// Gets a font with explicit DPI scale factor.
        /// </summary>
        public static Font GetFont(string fontName, float size, float dpiScaleFactor, FontStyle style = FontStyle.Regular)
        {
            float scaledSize = ApplyDpiScaling(size, dpiScaleFactor);
            return GetFontInternal(fontName, scaledSize, style);
        }

        /// <summary>
        /// Gets a font scaled for a specific control's DPI.
        /// </summary>
        public static Font GetFontForControl(string fontName, float size, Control control, FontStyle style = FontStyle.Regular)
        {
            float dpiScale = control != null ? DpiScalingHelper.GetDpiScaleFactor(control) : 1.0f;
            float scaledSize = ApplyDpiScaling(size, dpiScale);
            return GetFontInternal(fontName, scaledSize, style);
        }

        /// <summary>
        /// Internal method that gets a font without additional DPI scaling (size already scaled).
        /// </summary>
        private static Font GetFontInternal(string fontName, float size, FontStyle style = FontStyle.Regular)
        {
            EnsureInitialized();
            if (string.IsNullOrWhiteSpace(fontName))
            {
                fontName = "Segoe UI"; // Use default instead of Arial
            }

            string cacheKey = $"{fontName}|{size}|{(int)style}";
            
            return GetOrCreateFont(cacheKey, () =>
            {
                // Normalize known aliases (e.g., "segoeui" -> "Segoe UI")
                var normalized = NormalizeFontName(fontName);
                if (normalized == "segoeui" || normalized == "segoe")
                    fontName = "Segoe UI";

                // Try exact configuration match
                var fontConfig = FontConfigurations.FirstOrDefault(
                    f => f.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));

                // For system fonts, create from name directly (most reliable)
                if (fontConfig != null && fontConfig.IsSystemFont)
                {
                    return CreateValidFont(fontName, size, style);
                }
                
                // For private fonts, try by stored index, else by name lookup in private collection
                if (fontConfig != null && fontConfig.IsPrivateFont)
                {
                    if (fontConfig.PrivateFontIndex >= 0 && fontConfig.PrivateFontIndex < privateFontCollection.Families.Length)
                    {
                        var fam = privateFontCollection.Families[fontConfig.PrivateFontIndex];
                        if (fam != null)
                        {
                            return CreateValidFont(fam, size, style);
                        }
                    }

                    // fallback: search by name in private collection
                    var pfam = FindPrivateFamilyByName(fontName);
                    if (pfam != null)
                    {
                        return CreateValidFont(pfam, size, style);
                    }
                }

                // As a robust fallback: if some other config has close name match
                var alt = FontConfigurations.FirstOrDefault(f => NormalizeFontName(f.Name) == NormalizeFontName(fontName));
                if (alt != null)
                {
                    // system or private
                    if (alt.IsSystemFont)
                    {
                        return CreateValidFont(alt.Name, size, style);
                    }
                    var pfam2 = FindPrivateFamilyByName(alt.Name);
                    if (pfam2 != null)
                    {
                        return CreateValidFont(pfam2, size, style);
                    }
                }

                // Try resolving directly from the system font families on-demand
                var systemFamily = TryResolveSystemFamily(fontName);
                if (systemFamily != null)
                {
                    return CreateValidFont(systemFamily, size, style);
                }

                // Fallback: Try creating with original name (may work even if not in our config)
                var directFont = CreateValidFont(fontName, size, style);
                if (directFont != null)
                    return directFont;

                // Ultimate fallback
                return null; // GetOrCreateFont will call GetUltimateFallbackFont
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
        /// Tries to find a system-installed font family by name without relying on prior scans.
        /// </summary>
        private static FontFamily TryResolveSystemFamily(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            try
            {
                using (var installed = new InstalledFontCollection())
                {
                    var target = NormalizeFontName(name);
                    foreach (var fam in installed.Families)
                    {
                        if (NormalizeFontName(fam.Name) == target)
                            return fam;
                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Creates a font with validation and fallback handling
        /// </summary>
        private static Font CreateValidFont(string fontName, float size, FontStyle style)
        {
            try
            {
                var font = new Font(fontName, size, style);
                // Test that the font is fully accessible by accessing safe properties
                // DO NOT access font.Height as it can throw for certain fonts
                var _ = font.Size;
                var __ = font.Name;
                var ___ = font.FontFamily;
                var ____ = font.FontFamily.Name;
                return font;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a font with validation and fallback handling using FontFamily
        /// </summary>
        private static Font CreateValidFont(FontFamily family, float size, FontStyle style)
        {
            try
            {
                var font = new Font(family, size, style);
                // Test that the font is fully accessible by accessing safe properties
                // DO NOT access font.Height as it can.throw for certain fonts
                var _ = font.Size;
                var __ = font.Name;
                var ___ = font.FontFamily;
                var ____ = font.FontFamily.Name;
                return font;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Determines if a font is usable for drawing by measuring text height.
        /// Avoids accessing Font.Height which can throw for some private fonts.
        /// </summary>
        private static bool IsFontUsable(Font font)
        {
            try
            {
                if (font == null) return false;
                var size = TextRenderer.MeasureText("Ag", font, new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                return size.Height > 0 && size.Width > 0;
            }
            catch { return false; }
        }

        /// <summary>
        /// Public helper to get a safe font height without throwing.
        /// </summary>
        public static int GetFontHeightSafe(Font font, Control context = null)
        {
            try
            {
                if (font != null)
                {
                    var sz = TextRenderer.MeasureText("Ag", font, new Size(int.MaxValue, int.MaxValue),
                        TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                    if (sz.Height > 0) return sz.Height;
                }
            }
            catch { }
            try { return context?.Font?.Height ?? SystemFonts.DefaultFont.Height; } catch { return 12; }
        }

        /// <summary>
        /// Gets a font by index with specified size and Style.
        /// Applies DPI scaling if EnableDpiFontScaling is true.
        /// </summary>
        public static Font GetFontByIndex(int index, float size, FontStyle style = FontStyle.Regular)
        {
            if (index < 0 || index >= FontConfigurations.Count)
                return null;

            var fontConfig = FontConfigurations[index];
            return GetFont(fontConfig.Name, size, style);
        }

        /// <summary>
        /// Gets a font by index with explicit DPI scale factor.
        /// </summary>
        public static Font GetFontByIndex(int index, float size, float dpiScaleFactor, FontStyle style = FontStyle.Regular)
        {
            if (index < 0 || index >= FontConfigurations.Count)
                return null;

            var fontConfig = FontConfigurations[index];
            return GetFont(fontConfig.Name, size, dpiScaleFactor, style);
        }

        /// <summary>
        /// Gets a font by index scaled for a specific control's DPI.
        /// </summary>
        public static Font GetFontByIndexForControl(int index, float size, Control control, FontStyle style = FontStyle.Regular)
        {
            if (index < 0 || index >= FontConfigurations.Count)
                return null;

            var fontConfig = FontConfigurations[index];
            return GetFontForControl(fontConfig.Name, size, control, style);
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
        /// Gets a font with a fallback option if the primary font is not available.
        /// Applies DPI scaling if EnableDpiFontScaling is true.
        /// </summary>
        public static Font GetFontWithFallback(string primaryFontName, string fallbackFontName, float size, FontStyle style = FontStyle.Regular)
        {
            EnsureInitialized();
            
            // Try to get the primary font - GetFont handles all the caching and validation
            Font primaryFont = GetFont(primaryFontName, size, style);
            
            // GetFont never returns null (it has GetUltimateFallbackFont), so we're done
            // But we can check if we got what we wanted
            if (primaryFont != null && 
                NormalizeFontName(primaryFont.FontFamily.Name) == NormalizeFontName(primaryFontName))
            {
                return primaryFont;
            }

            // If primary didn't match, try fallback explicitly
            if (!string.IsNullOrWhiteSpace(fallbackFontName) && 
                !fallbackFontName.Equals(primaryFontName, StringComparison.OrdinalIgnoreCase))
            {
                Font fallbackFont = GetFont(fallbackFontName, size, style);
                if (fallbackFont != null)
                {
                    return fallbackFont;
                }
            }

            // Return whatever GetFont gave us (it's guaranteed valid)
            return primaryFont;
        }

        /// <summary>
        /// Gets a font with a fallback option scaled for a specific control's DPI.
        /// </summary>
        public static Font GetFontWithFallbackForControl(string primaryFontName, string fallbackFontName, float size, Control control, FontStyle style = FontStyle.Regular)
        {
            EnsureInitialized();
            
            Font primaryFont = GetFontForControl(primaryFontName, size, control, style);
            
            if (primaryFont != null && 
                NormalizeFontName(primaryFont.FontFamily.Name) == NormalizeFontName(primaryFontName))
            {
                return primaryFont;
            }

            if (!string.IsNullOrWhiteSpace(fallbackFontName) && 
                !fallbackFontName.Equals(primaryFontName, StringComparison.OrdinalIgnoreCase))
            {
                Font fallbackFont = GetFontForControl(fallbackFontName, size, control, style);
                if (fallbackFont != null)
                {
                    return fallbackFont;
                }
            }

            return primaryFont;
        }

        /// <summary>
        /// Safely gets a font from cache or creates a new one. Never returns null or disposed fonts.
        /// </summary>
        private static Font GetOrCreateFont(string cacheKey, Func<Font> fontFactory)
        {
            lock (fontCacheLock)
            {
                // Try to get from cache
                if (fontCache.TryGetValue(cacheKey, out var cachedFont))
                {
                    // Validate the cached font is still usable
                    try
                    {
                        // Test if font is valid by accessing properties
                        var _ = cachedFont.Size;
                        var __ = cachedFont.Name;
                        return cachedFont;
                    }
                    catch
                    {
                        // Font is disposed or invalid, remove from cache
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
                            var __ = newFont.Name; // Test if valid
                            fontCache[cacheKey] = newFont;
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
            // Try standard fallback fonts using CreateValidFont directly to avoid recursion
            return CreateValidFont("Segoe UI", 9f, FontStyle.Regular) ??
                   CreateValidFont("Arial", 9f, FontStyle.Regular) ??
                   CreateValidFont(FontFamily.GenericSansSerif, 9f, FontStyle.Regular) ??
                   CreateValidFont("Microsoft Sans Serif", 9f, FontStyle.Regular) ??
                   SystemFonts.DefaultFont;
        }

        public static FontFamily GetFontFamily(string fontFamily)
        {
            return FontConfigurations
                .Where(f => f.Name.Equals(fontFamily, StringComparison.OrdinalIgnoreCase))
                .Select(f =>
                {
                    if (f.IsSystemFont)
                    {
                        try { return new FontFamily(f.Name); } catch { return null; }
                    }
                    else if (f.IsPrivateFont)
                    {
                        if (f.PrivateFontIndex >= 0 && f.PrivateFontIndex < privateFontCollection.Families.Length)
                        {
                            return privateFontCollection.Families[f.PrivateFontIndex];
                        }
                        else
                        {
                            return FindPrivateFamilyByName(f.Name);
                        }
                    }
                    return null;
                })
                .FirstOrDefault(fam => fam != null);

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
