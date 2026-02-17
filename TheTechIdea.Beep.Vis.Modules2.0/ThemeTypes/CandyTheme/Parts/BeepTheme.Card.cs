using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Card Colors & Fonts

        public TypographyStyle CardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 14f, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.FromArgb(85, 85, 85); // Neutral gray for good contrast

        public Color CardBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel pink for main card area

        public Color CardTitleForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        public TypographyStyle CardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold | FontStyle.Italic);
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint

        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 13,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(240, 100, 180),
            LetterSpacing = 0.1f,
            LineHeight = 1.2f
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(85, 85, 85),
            LetterSpacing = 0f,
            LineHeight = 1.3f
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(127, 255, 212),
            LetterSpacing = 0.05f,
            LineHeight = 1.1f
        };

        // Gradient from pink to mint with a lemon highlight in the middle
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(204, 255, 240);   // Pastel Mint
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
