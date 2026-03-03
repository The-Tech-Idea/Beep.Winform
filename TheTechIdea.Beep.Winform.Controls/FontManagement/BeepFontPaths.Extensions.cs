using System.Reflection;

namespace TheTechIdea.Beep.Winform.Controls.FontManagement
{
    /// <summary>
    /// Utilities partial class containing extension methods and helper functions.
    /// </summary>
    public static partial class BeepFontPaths
    {
        // ── Runtime-registered families from external assemblies ────────────────
        // Populated by RegisterFamilyFromAssembly(); merged into GetAllFamilies().
        private static readonly Dictionary<string, Dictionary<string, string>> _runtimeFamilies
            = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets all font resource paths grouped by font family.
        /// Includes both the built-in Controls-assembly families AND any families registered at
        /// runtime via <see cref="RegisterFamilyFromAssembly"/>.
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> GetAllFamilies()
        {
            var families = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                // Web Fonts
                { "Cairo", Families.Cairo.GetAllWeights() },
                { "Roboto", Families.Roboto.GetAllStyles() },
                { "RobotoCondensed", Families.RobotoCondensed.GetAllStyles() },
                { "OpenSans", Families.OpenSans.GetAllStyles() },
                { "Montserrat", Families.Montserrat.GetAllStyles() },
                { "Inter", Families.Inter.GetAllStyles() },
                { "Nunito", Families.Nunito.GetAllStyles() },
                { "Tajawal", Families.Tajawal.GetAllWeights() },
                { "Oxygen", Families.Oxygen.GetAllStyles() },

                // Monospace Fonts
                { "FiraCode", Families.FiraCode.GetAllWeights() },
                { "JetBrainsMono", Families.JetBrainsMono.GetAllStyles() },
                { "CascadiaCode", Families.CascadiaCode.GetAllStyles() },
                { "SpaceMono", Families.SpaceMono.GetAllStyles() },
                { "DejaVuSansMono", Families.DejaVuSansMono.GetAllStyles() },

                // Display Fonts
                { "BebasNeue", Families.BebasNeue.GetAllWeights() },
                { "Orbitron", Families.Orbitron.GetAllWeights() },
                { "Exo2", Families.Exo2.GetAllStyles() },
                { "Rajdhani", Families.Rajdhani.GetAllWeights() },
                { "Whitney", Families.Whitney.GetAllStyles() },
                { "NotoColorEmoji", Families.NotoColorEmoji.GetAllStyles() },

                // Accessibility Fonts
                { "AtkinsonHyperlegible", Families.AtkinsonHyperlegible.GetAllStyles() },
                { "OpenDyslexic", Families.OpenDyslexic.GetAllStyles() },
                { "Lexend", Families.Lexend.GetAllWeights() },

                // System Fonts
                { "SourceSansPro", Families.SourceSansPro.GetAllStyles() },
                { "ComicNeue", Families.ComicNeue.GetAllStyles() },

                // Individual Fonts
                { "Individual", new Dictionary<string, string>
                    {
                        { "Caprasimo", Families.Individual.Caprasimo },
                        { "Consolas", Families.Individual.Consolas }
                    }
                }
            };

            // Merge runtime-registered families (from external assemblies / plugins)
            foreach (var kvp in _runtimeFamilies)
            {
                if (!families.TryGetValue(kvp.Key, out var existing))
                {
                    families[kvp.Key] = new Dictionary<string, string>(kvp.Value, StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    foreach (var styleKvp in kvp.Value)
                        existing[styleKvp.Key] = styleKvp.Value;
                }
            }

            return families;
        }

        /// <summary>
        /// Scans an assembly's embedded font resources under the given namespace root and adds the
        /// discovered families to the runtime lookup table so that <see cref="GetFontPath"/>,
        /// <see cref="GetFontFamilyNames"/>, and related look-up methods return them.
        /// </summary>
        /// <param name="assembly">Assembly to scan. Null is silently ignored.</param>
        /// <param name="namespaceRoot">
        /// Namespace root under which fonts are embedded (e.g. "MyApp.Fonts").
        /// Pass null to accept all embedded .ttf/.otf resources in the assembly.
        /// </param>
        public static void RegisterFamilyFromAssembly(Assembly assembly, string namespaceRoot = null)
        {
            if (assembly == null || assembly.IsDynamic) return;
            try
            {
                foreach (var res in assembly.GetManifestResourceNames())
                {
                    var ext = System.IO.Path.GetExtension(res)?.ToLowerInvariant();
                    if (ext != ".ttf" && ext != ".otf") continue;

                    if (!string.IsNullOrWhiteSpace(namespaceRoot) &&
                        !res.StartsWith(namespaceRoot, StringComparison.OrdinalIgnoreCase))
                        continue;

                    string familyName = ExtractFontNameFromPath(res);
                    if (string.IsNullOrWhiteSpace(familyName)) continue;

                    string styleStr = ExtractStyleFromResourcePath(res);

                    if (!_runtimeFamilies.TryGetValue(familyName, out var styles))
                    {
                        styles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        _runtimeFamilies[familyName] = styles;
                    }
                    styles[styleStr] = res;
                }
            }
            catch { /* ignore */ }
        }

        /// <summary>
        /// Infers a style string ("Regular", "Bold", "Italic", "BoldItalic", "Light", etc.)
        /// from an embedded resource path based on filename suffix conventions.
        /// </summary>
        private static string ExtractStyleFromResourcePath(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath)) return "Regular";
            var name = System.IO.Path.GetFileNameWithoutExtension(resourcePath);
            if (name.EndsWith("-BoldItalic", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith("BoldItalic", StringComparison.OrdinalIgnoreCase)) return "BoldItalic";
            if (name.EndsWith("-Bold", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith("Bold", StringComparison.OrdinalIgnoreCase)) return "Bold";
            if (name.EndsWith("-Italic", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith("Italic", StringComparison.OrdinalIgnoreCase)) return "Italic";
            if (name.EndsWith("-Light", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith("Light", StringComparison.OrdinalIgnoreCase)) return "Light";
            if (name.EndsWith("-Medium", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith("Medium", StringComparison.OrdinalIgnoreCase)) return "Medium";
            if (name.EndsWith("-SemiBold", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith("SemiBold", StringComparison.OrdinalIgnoreCase)) return "SemiBold";
            return "Regular";
        }
    }

    /// <summary>
    /// Extension methods for easier use of BeepFontPaths with font management.
    /// </summary>
    public static class BeepFontPathsExtensions
    {
        /// <summary>
        /// Loads a font from embedded resources and adds it to the FontListHelper.
        /// </summary>
        /// <param name="fontResourcePath">The font resource path from BeepFontPaths</param>
        /// <returns>True if the font was loaded successfully</returns>
        public static bool LoadFontFromEmbeddedResource(string fontResourcePath)
        {
            if (string.IsNullOrEmpty(fontResourcePath))
                throw new ArgumentNullException(nameof(fontResourcePath));

            try
            {
                // Use the existing FontListHelper to load embedded resources
                var fontNamespaces = new[] { "TheTechIdea.Beep.Winform.Controls.Fonts" };
                var loadedFonts = FontListHelper.GetFontResourcesFromEmbedded(fontNamespaces);

                return loadedFonts.Any(f => f.Path == fontResourcePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load font resource: {fontResourcePath}", ex);
            }
        }

        /// <summary>
        /// Creates a Font object from a BeepFontPaths resource.
        /// </summary>
        /// <param name="fontResourcePath">The font resource path from BeepFontPaths</param>
        /// <param name="size">Font size</param>
        /// <param name="style">Font Style</param>
        /// <returns>Font object or null if failed</returns>
        public static Font CreateFontFromResource(string fontResourcePath, float size, FontStyle style = FontStyle.Regular)
        {
            if (string.IsNullOrEmpty(fontResourcePath))
                return null;

            try
            {
                // First ensure the font is loaded
                LoadFontFromEmbeddedResource(fontResourcePath);

                // Extract the font name from the path
                string fontName = BeepFontPaths.ExtractFontNameFromPath(fontResourcePath);

                // Use FontListHelper to get the font
                return FontListHelper.GetFont(fontName, size, style);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a font using the BeepFontPaths system with fallback.
        /// </summary>
        /// <param name="familyName">Font family name</param>
        /// <param name="style">Font Style</param>
        /// <param name="size">Font size</param>
        /// <param name="fallbackFontName">Fallback font name if primary not found</param>
        /// <returns>Font object</returns>
        public static Font GetBeepFont(string familyName, string style, float size, string fallbackFontName = "Arial")
        {
            string fontPath = BeepFontPaths.GetFontPath(familyName, style);
            if (!string.IsNullOrEmpty(fontPath))
            {
                var font = CreateFontFromResource(fontPath, size);
                if (font != null)
                    return font;
            }

            // Fallback to system font
            return FontListHelper.GetFontWithFallback(familyName, fallbackFontName, size);
        }
    }
}