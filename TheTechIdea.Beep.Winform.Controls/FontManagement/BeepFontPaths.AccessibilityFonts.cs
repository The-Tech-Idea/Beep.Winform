using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.FontManagement
{
    /// <summary>
    /// Accessibility Fonts partial class containing accessibility-focused fonts like Atkinson Hyperlegible, OpenDyslexic, Lexend.
    /// </summary>
    public static partial class BeepFontPaths
    {
        #region "Atkinson Hyperlegible Font Family"
        private const string AtkinsonHyperlegibleNamespace = BaseNamespace + ".Atkinson_Hyperlegible.atkinson-hyperlegible-main.fonts.ttf";
        public static readonly string AtkinsonHyperlegibleBold = $"{AtkinsonHyperlegibleNamespace}.AtkinsonHyperlegible-Bold.ttf";
        public static readonly string AtkinsonHyperlegibleBoldItalic = $"{AtkinsonHyperlegibleNamespace}.AtkinsonHyperlegible-BoldItalic.ttf";
        public static readonly string AtkinsonHyperlegibleItalic = $"{AtkinsonHyperlegibleNamespace}.AtkinsonHyperlegible-Italic.ttf";
        public static readonly string AtkinsonHyperlegibleRegular = $"{AtkinsonHyperlegibleNamespace}.AtkinsonHyperlegible-Regular.ttf";
        #endregion

        #region "OpenDyslexic Font Family"
        private const string OpenDyslexicNamespace = BaseNamespace + ".OpenDyslexic.open-dyslexic-master.otf";
        public static readonly string OpenDyslexicBold = $"{OpenDyslexicNamespace}.OpenDyslexic-Bold.otf";
        public static readonly string OpenDyslexicBoldItalic = $"{OpenDyslexicNamespace}.OpenDyslexic-BoldItalic.otf";
        public static readonly string OpenDyslexicItalic = $"{OpenDyslexicNamespace}.OpenDyslexic-Italic.otf";
        public static readonly string OpenDyslexicRegular = $"{OpenDyslexicNamespace}.OpenDyslexic-Regular.otf";
        #endregion

        #region "Lexend Font Family"
        private const string LexendNamespace = BaseNamespace + ".Lexend.lexend-main.fonts.lexend.ttf";
        public static readonly string LexendBlack = $"{LexendNamespace}.Lexend-Black.ttf";
        public static readonly string LexendBold = $"{LexendNamespace}.Lexend-Bold.ttf";
        public static readonly string LexendExtraBold = $"{LexendNamespace}.Lexend-ExtraBold.ttf";
        public static readonly string LexendExtraLight = $"{LexendNamespace}.Lexend-ExtraLight.ttf";
        public static readonly string LexendLight = $"{LexendNamespace}.Lexend-Light.ttf";
        public static readonly string LexendMedium = $"{LexendNamespace}.Lexend-Medium.ttf";
        public static readonly string LexendRegular = $"{LexendNamespace}.Lexend-Regular.ttf";
        public static readonly string LexendSemiBold = $"{LexendNamespace}.Lexend-SemiBold.ttf";
        public static readonly string LexendThin = $"{LexendNamespace}.Lexend-Thin.ttf";
        #endregion

        /// <summary>
        /// Resolves a known accessibility font family/style to an embedded Beep font resource path.
        /// </summary>
        public static string GetAccessibilityFontPath(string familyName, FontStyle style = FontStyle.Regular)
        {
            if (string.IsNullOrWhiteSpace(familyName))
                return string.Empty;

            var key = familyName.Replace(" ", string.Empty).Trim();
            var styleKey = style switch
            {
                FontStyle.Bold => "Bold",
                FontStyle.Italic => "Italic",
                FontStyle.Bold | FontStyle.Italic => "BoldItalic",
                _ => "Regular"
            };

            return key switch
            {
                "AtkinsonHyperlegible" => styleKey switch
                {
                    "Bold" => AtkinsonHyperlegibleBold,
                    "Italic" => AtkinsonHyperlegibleItalic,
                    "BoldItalic" => AtkinsonHyperlegibleBoldItalic,
                    _ => AtkinsonHyperlegibleRegular
                },
                "OpenDyslexic" => styleKey switch
                {
                    "Bold" => OpenDyslexicBold,
                    "Italic" => OpenDyslexicItalic,
                    "BoldItalic" => OpenDyslexicBoldItalic,
                    _ => OpenDyslexicRegular
                },
                "Lexend" => styleKey switch
                {
                    "Bold" => LexendBold,
                    _ => LexendRegular
                },
                _ => string.Empty
            };
        }
    }
}
