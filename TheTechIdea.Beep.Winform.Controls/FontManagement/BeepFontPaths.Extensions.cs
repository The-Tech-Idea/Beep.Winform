namespace TheTechIdea.Beep.Winform.Controls.FontManagement
{
    /// <summary>
    /// Utilities partial class containing extension methods and helper functions.
    /// </summary>
    public static partial class BeepFontPaths
    {
        /// <summary>
        /// Gets all font resource paths grouped by font family.
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> GetAllFamilies()
        {
            return new Dictionary<string, Dictionary<string, string>>
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