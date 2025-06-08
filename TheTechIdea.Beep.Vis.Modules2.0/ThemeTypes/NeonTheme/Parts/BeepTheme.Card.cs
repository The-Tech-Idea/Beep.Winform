using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Card Colors & Fonts
        public TypographyStyle CardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 18, FontStyle.Bold); // Bold, prominent title
        public Color CardTextForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for readable text
        public Color CardBackColor { get; set; } = Color.FromArgb(44, 62, 80); // Dark background for neon contrast
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for titles
        public TypographyStyle CardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14, FontStyle.Regular); // Lighter subtitle
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for subtitles
        public TypographyStyle CardHeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 20,
            LineHeight = 1.4f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle CardParagraphFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14,
            LineHeight = 1.6f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        //public TypographyStyle CardSubTitleFont { get; set; } = new TypographyStyle
        //{
        //    FontFamily = "Roboto",
        //    FontSize = 16,
        //    LineHeight = 1.5f,
        //    LetterSpacing = 0.3f,
        //    FontWeight = FontWeight.Medium,
        //    FontStyle = FontStyle.Regular,
        //    TextColor = Color.FromArgb(46, 204, 113), // Neon green
        //    IsUnderlined = false,
        //    IsStrikeout = false
        //};
        public Color CardGradiantStartColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(142, 68, 173); // Neon purple
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical gradient for a sleek look
    }
}