using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Card Colors & Fonts
//<<<<<<< HEAD
        public TypographyStyle  CardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.Black;
        public Color CardBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color CardTitleForeColor { get; set; } = Color.Black;

        public TypographyStyle  CardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);
        public Color CardSubTitleForeColor { get; set; } = Color.DimGray;

        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Black,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.DarkSlateGray,
            LineHeight = 1.4f,
            LetterSpacing = 0f
        };

        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Italic,
            TextColor = Color.DimGray,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(215, 230, 245);
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
