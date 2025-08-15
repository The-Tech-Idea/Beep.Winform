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

namespace TheTechIdea.Beep.Vis.Modules.Managers
{
      static class FontListHelper
    {
        // Static references
      

        // This tracks all discovered fonts (system, local or embedded)
        public static List<FontConfiguration> FontConfigurations { get; set; } = new List<FontConfiguration>();

        // A collection to manage private fonts
        private static PrivateFontCollection privateFontCollection = new PrivateFontCollection();

        // A simple incremental index
        private static int index = -1;

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
                            Index = index++,
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
                .Concat(Directory.GetFiles(path, "*.otf", SearchOption.AllDirectories)).ToArray();

            foreach (string file in fontFiles)
            {
                string filename = Path.GetFileName(file);
                
                // Check if already in FontConfigurations
                bool alreadyInList = FontConfigurations.Any(
                    cfg => cfg.Path == file);

                if (!alreadyInList)
                {
                    try
                    {
                        // Add font to private font collection
                        privateFontCollection.AddFontFile(file);
                        
                        // Get the index of the last added font
                        int lastIndex = privateFontCollection.Families.Length - 1;
                        FontFamily fontFamily = privateFontCollection.Families[lastIndex];

                        var config = new FontConfiguration
                        {
                            Index = index++,
                            Name = fontFamily.Name,
                            Path = file,
                            FileName = filename,
                            IsSystemFont = false,
                            IsPrivateFont = true,
                            IsEmbeddedResource = false,
                            PrivateFontIndex = lastIndex,
                            StylesAvailable = GetAvailableStyles(fontFamily)
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
        /// <summary>
        /// Scans assemblies for embedded font resources (.ttf, .otf),
        /// creates FontConfiguration for each, and adds them to FontConfigurations.
        /// </summary>
        /// <param name="namespaces">Optional array of namespace strings to filter resources</param>
        /// <returns>A list of newly discovered FontConfiguration objects</returns>
        public static List<FontConfiguration> GetFontResourcesFromEmbedded(string[] namespaces = null)
        {
            var result = new List<FontConfiguration>();

            // Get all assemblies
            var assemblies = new List<Assembly>
            {
                Assembly.GetExecutingAssembly(),
                Assembly.GetCallingAssembly(),
                Assembly.GetEntryAssembly()!
            };

            

            assemblies = assemblies.Where(a => a != null).Distinct().ToList();

            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    string[] resources = assembly.GetManifestResourceNames();

                    foreach (string resource in resources)
                    {
                        // Check if resource matches namespace filter if provided
                        if (namespaces != null && namespaces.Length > 0)
                        {
                            if (!namespaces.Any(ns => resource.StartsWith(ns, StringComparison.OrdinalIgnoreCase)))
                                continue;
                        }

                     
                        string extension = Path.GetExtension(resource)?.ToLowerInvariant();
                        if (extension != ".ttf" && extension != ".otf" &&
                            extension != ".woff" && extension != ".woff2")
                            continue;

                        // Check if already in our list
                        bool alreadyInList = FontConfigurations.Any(
                            cfg => cfg.Path == resource && cfg.AssemblyFullName == assembly.FullName);

                        if (!alreadyInList)
                        {
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
                                        FontFamily fontFamily = privateFontCollection.Families[lastIndex];

                                        string fontName = fontFamily.Name;

                                        var config = new FontConfiguration
                                        {
                                            Index = index++,
                                            Name = fontName,
                                            Path = resource,
                                            FileName = Path.GetFileName(resource),
                                            IsSystemFont = false,
                                            IsPrivateFont = true,
                                            IsEmbeddedResource = true,
                                            AssemblyFullName = assembly.FullName,
                                            AssemblyLocation = assembly.Location,
                                            PrivateFontIndex = lastIndex,
                                            StylesAvailable = GetAvailableStyles(fontFamily)
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error searching for fonts in assembly {assembly.FullName}: {ex.Message}");
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
        /// Gets a font by name with specified size and style
        /// </summary>
        /// <param name="fontName">Name of the font to retrieve</param>
        /// <param name="size">Size of the font</param>
        /// <param name="style">Style of the font</param>
        /// <returns>A Font object, or null if not found</returns>
        public static Font GetFont(string fontName, float size, FontStyle style = FontStyle.Regular)
        {
            if (string.IsNullOrEmpty(fontName))
            {
                // Fallback to Arial if no font name is provided
                return new Font("Arial", size, style);
            }

            var fontConfig = FontConfigurations.FirstOrDefault(
                f => f.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));

            try
            {
                // For system fonts, create from name
                if (fontConfig != null && fontConfig.IsSystemFont)
                {
                    return new Font(fontName, size, style);
                }
                // For private fonts, create from private font collection
                else if (fontConfig != null && fontConfig.IsPrivateFont && fontConfig.PrivateFontIndex >= 0)
                {
                    if (fontConfig.PrivateFontIndex < privateFontCollection.Families.Length)
                    {
                        return new Font(privateFontCollection.Families[fontConfig.PrivateFontIndex], size, style);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create font {fontName}: {ex.Message}");
            }

            // Fallback: Try Arial, then GenericSansSerif
            try
            {
                return new Font("Arial", size, style);
            }
            catch
            {
                try
                {
                    return new Font(FontFamily.GenericSansSerif, size, style);
                }
                catch
                {
                    // As a last resort, return null (should be extremely rare)
                    return null;
                }
            }
        }
        /// <summary>
        /// Gets a font by index with specified size and style
        /// </summary>
        /// <param name="index">RowIndex of the font in FontConfigurations</param>
        /// <param name="size">Size of the font</param>
        /// <param name="style">Style of the font</param>
        /// <returns>A Font object, or null if not found</returns>
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
        /// <param name="fontName">Name of the font to find</param>
        /// <returns>RowIndex of the font in FontConfigurations, or -1 if not found</returns>
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
        /// <returns>List of font names</returns>
        public static List<string> GetFontNames()
        {
            return FontConfigurations.Select(f => f.Name).ToList();
        }

        /// <summary>
        /// Gets a list of system font names
        /// </summary>
        /// <returns>List of system font names</returns>
        public static List<string> GetSystemFontNames()
        {
            return FontConfigurations.Where(f => f.IsSystemFont).Select(f => f.Name).ToList();
        }

        /// <summary>
        /// Gets a list of private font names
        /// </summary>
        /// <returns>List of private font names</returns>
        public static List<string> GetPrivateFontNames()
        {
            return FontConfigurations.Where(f => f.IsPrivateFont).Select(f => f.Name).ToList();
        }

        /// <summary>
        /// Gets a font with a fallback option if the primary font is not available
        /// </summary>
        /// <param name="primaryFontName">First choice font name</param>
        /// <param name="fallbackFontName">Fallback font name</param>
        /// <param name="size">Font size</param>
        /// <param name="style">Font style</param>
        /// <returns>A font object using either the primary or fallback font</returns>
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
