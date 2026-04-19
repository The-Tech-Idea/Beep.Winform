using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Card Colors & Fonts
        public TypographyStyle CardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.DarkGreen;
        public Color CardBackColor { get; set; } = Color.FromArgb(230, 245, 230); // Light greenish background
        public Color CardTitleForeColor { get; set; } = Color.ForestGreen;
        public TypographyStyle CardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);
        public Color CardSubTitleForeColor { get; set; } = Color.SeaGreen;
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextColor = Color.ForestGreen
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.Normal,
            TextColor = Color.DarkGreen
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.SeaGreen
        };
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(200, 230, 200);
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(170, 210, 170);
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(185, 220, 185);
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
