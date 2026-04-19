using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Card Colors & Fonts - Desert Warmth
        public TypographyStyle CardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI Semibold", 14f, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.FromArgb(92, 64, 51); // Dark Sand Brown
        public Color CardBackColor { get; set; } = Color.FromArgb(255, 244, 214); // Light Sand
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(150, 75, 0); // Saddle Brown
        public TypographyStyle CardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna

        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI Semibold",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(150, 75, 0),
            LineHeight = 1.2f
        };

        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(92, 64, 51),
            LineHeight = 1.4f
        };

        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI Italic",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(160, 82, 45),
            LineHeight = 1.3f
        };

        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(255, 244, 214); // Light Sand
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(241, 208, 160);   // Soft Clay
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(227, 184, 138); // Warm Tan
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
