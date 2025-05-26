using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {

        public TypographyStyle  CardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.FromArgb(220, 220, 240); // Light lavender text
        public Color CardBackColor { get; set; } = Color.FromArgb(25, 25, 50); // Deep space blue
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(180, 180, 255); // Light blue-purple
        public TypographyStyle  CardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Italic);
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(160, 160, 210); // Medium lavender
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(200, 200, 255)
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(220, 220, 240)
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(160, 160, 210)
        };
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(20, 20, 40); // Darker blue
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(35, 35, 70); // Lighter blue
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(28, 28, 55); // Mid-tone blue
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.ForwardDiagonal;

    }
}