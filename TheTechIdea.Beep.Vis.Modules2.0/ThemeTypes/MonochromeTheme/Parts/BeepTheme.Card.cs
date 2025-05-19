using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Card Colors & Fonts
        public TypographyStyle CardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.Silver;
        public Color CardBackColor { get; set; } = Color.Black;
        public Color CardTitleForeColor { get; set; } = Color.WhiteSmoke;
        public TypographyStyle CardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Italic);
        public Color CardSubTitleForeColor { get; set; } = Color.Gray;

        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16F,
            FontWeight = FontWeight.Bold,
            TextColor = Color.WhiteSmoke,
            LineHeight = 1.3f,
            LetterSpacing = 0f,
            FontStyle = FontStyle.Regular
        };

        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12F,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Silver,
            LineHeight = 1.2f,
            LetterSpacing = 0f,
            FontStyle = FontStyle.Regular
        };

        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10F,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray,
            LineHeight = 1.1f,
            LetterSpacing = 0f,
            FontStyle = FontStyle.Italic
        };

        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color CardGradiantEndColor { get; set; } = Color.Black;
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(70, 70, 70);
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
