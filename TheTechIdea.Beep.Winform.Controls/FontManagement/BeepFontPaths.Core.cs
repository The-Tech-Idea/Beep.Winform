using System.Drawing.Text;
using System.IO;
using System.Reflection;

namespace TheTechIdea.Beep.Winform.Controls.FontManagement
{
    /// <summary>
    /// Core partial class for BeepFontPaths containing constants, utilities, and base functionality.
    /// Static class providing easy access to all embedded font resource paths in the Beep.Winform.Controls assembly.
    /// All paths are formatted as embedded resource names for use with Assembly.GetManifestResourceStream().
    /// </summary>
    public static partial class BeepFontPaths
    {
        private const string BaseNamespace = "TheTechIdea.Beep.Winform.Controls.Fonts";

        /// <summary>
        /// Gets the assembly containing the embedded font resources.
        /// </summary>
        public static Assembly ResourceAssembly => Assembly.GetExecutingAssembly();

        #region "Individual Font Files"
        public static readonly string CaprasimoRegular = $"{BaseNamespace}.Caprasimo-Regular.ttf";
        public static readonly string Consolas = $"{BaseNamespace}.consolas.ttf";
        #endregion

        /// <summary>
        /// Gets all font resource paths as a dictionary for easy enumeration.
        /// </summary>
        public static Dictionary<string, string> GetAllPaths()
        {
            var paths = new Dictionary<string, string>();
            var type = typeof(BeepFontPaths);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(string) && field.IsLiteral == false && field.IsInitOnly)
                {
                    var value = field.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(value) && (value.EndsWith(".ttf") || value.EndsWith(".otf")))
                    {
                        paths[field.Name] = value;
                    }
                }
            }

            return paths;
        }

        /// <summary>
        /// Checks if a font resource path exists in the assembly.
        /// </summary>
        /// <param name="resourcePath">The full resource path</param>
        /// <returns>True if the resource exists</returns>
        public static bool ResourceExists(string resourcePath)
        {
            var resourceNames = ResourceAssembly.GetManifestResourceNames();
            return resourceNames.Contains(resourcePath);
        }

        /// <summary>
        /// Gets all available font resource names from the assembly.
        /// </summary>
        /// <returns>Array of resource names</returns>
        public static string[] GetAvailableFontResources()
        {
            return ResourceAssembly.GetManifestResourceNames()
                .Where(name => name.StartsWith(BaseNamespace) && (name.EndsWith(".ttf") || name.EndsWith(".otf")))
                .ToArray();
        }

        /// <summary>
        /// Helper method to get the full file system path (useful for development/debugging).
        /// This assumes the standard project structure.
        /// </summary>
        /// <param name="fontFileName">Just the font filename (e.g., "Roboto-Regular.ttf")</param>
        /// <param name="subfolder">Optional subfolder (e.g., "Roboto", "Cairo")</param>
        /// <returns>Full file system path</returns>
        public static string GetFileSystemPath(string fontFileName, string subfolder = null)
        {
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(subfolder))
            {
                return Path.Combine(baseDirectory, "..", "..", "..", "Fonts", fontFileName);
            }
            else
            {
                return Path.Combine(baseDirectory, "..", "..", "..", "Fonts", subfolder, fontFileName);
            }
        }

        /// <summary>
        /// Gets the best matching font path for a given font family and style.
        /// </summary>
        /// <param name="familyName">Font family name (e.g., "Roboto", "Cairo")</param>
        /// <param name="style">Font style (e.g., "Bold", "Italic", "Regular")</param>
        /// <returns>Font resource path if found, null otherwise</returns>
        public static string GetFontPath(string familyName, string style = "Regular")
        {
            var families = GetAllFamilies();

            if (families.TryGetValue(familyName, out var familyFonts))
            {
                if (familyFonts.TryGetValue(style, out var fontPath))
                {
                    return fontPath;
                }

                // Fallback to Regular if requested style not found
                if (style != "Regular" && familyFonts.TryGetValue("Regular", out var regularPath))
                {
                    return regularPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all available font families.
        /// </summary>
        /// <returns>List of font family names</returns>
        public static List<string> GetFontFamilyNames()
        {
            return GetAllFamilies().Keys.ToList();
        }

        /// <summary>
        /// Gets all available styles for a specific font family.
        /// </summary>
        /// <param name="familyName">Font family name</param>
        /// <returns>List of available styles for the family</returns>
        public static List<string> GetFontFamilyStyles(string familyName)
        {
            var families = GetAllFamilies();
            if (families.TryGetValue(familyName, out var familyFonts))
            {
                return familyFonts.Keys.ToList();
            }
            return new List<string>();
        }

        /// <summary>
        /// Helper method to extract font name from resource path.
        /// This is a basic implementation - you might need to adjust based on your specific requirements.
        /// </summary>
        /// <param name="fontResourcePath">Font resource path</param>
        /// <returns>Extracted font name</returns>
        public static string ExtractFontNameFromPath(string fontResourcePath)
        {
            if (string.IsNullOrEmpty(fontResourcePath))
                return string.Empty;

            // Extract filename without extension
            string fileName = Path.GetFileNameWithoutExtension(fontResourcePath);

            // Handle common naming conventions
            if (fileName.StartsWith("Cairo-"))
                return "Cairo";
            if (fileName.StartsWith("Roboto"))
                return "Roboto";
            if (fileName.StartsWith("ComicNeue"))
                return "Comic Neue";
            if (fileName.StartsWith("SourceSans"))
                return "Source Sans Pro";
            if (fileName.StartsWith("SpaceMono"))
                return "Space Mono";
            if (fileName.StartsWith("Tajawal"))
                return "Tajawal";
            if (fileName.StartsWith("BebasNeue"))
                return "Bebas Neue";
            if (fileName.StartsWith("CascadiaCode") || fileName.StartsWith("CascadiaMono"))
                return "Cascadia Code";
            if (fileName.StartsWith("DejaVuSansMono"))
                return "DejaVu Sans Mono";
            if (fileName.StartsWith("Exo2"))
                return "Exo 2";
            if (fileName.StartsWith("OpenDyslexic"))
                return "OpenDyslexic";
            if (fileName.StartsWith("orbitron") || fileName.Contains("Orbitron"))
                return "Orbitron";
            if (fileName.StartsWith("Rajdhani"))
                return "Rajdhani";
            if (fileName.Contains("Caprasimo"))
                return "Caprasimo";
            if (fileName.Contains("consolas"))
                return "Consolas";
            if (fileName.StartsWith("Inter"))
                return "Inter";
            if (fileName.StartsWith("JetBrainsMono"))
                return "JetBrains Mono";
            if (fileName.StartsWith("FiraCode"))
                return "Fira Code";
            if (fileName.StartsWith("Montserrat"))
                return "Montserrat";
            if (fileName.StartsWith("OpenSans"))
                return "Open Sans";
            if (fileName.StartsWith("AtkinsonHyperlegible"))
                return "Atkinson Hyperlegible";
            if (fileName.StartsWith("Lexend"))
                return "Lexend";
            if (fileName.StartsWith("Nunito"))
                return "Nunito";
            if (fileName.StartsWith("NotoColorEmoji"))
                return "Noto Color Emoji";
            if (fileName.StartsWith("Oxygen"))
                return "Oxygen";
            if (fileName.StartsWith("whitney") || fileName.StartsWith("Whitney"))
                return "Whitney";

            return fileName;
        }
    }
}