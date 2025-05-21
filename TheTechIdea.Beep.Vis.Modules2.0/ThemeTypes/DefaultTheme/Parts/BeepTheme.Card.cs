using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Card Colors & Fonts
        public TypographyStyle CardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16F, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Gray
        public Color CardBackColor { get; set; } = Color.White;
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Gray
        public TypographyStyle CardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Medium Gray

        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(33, 33, 33),
            LineHeight = 1.15f
        };

        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(66, 66, 66),
            LineHeight = 1.1f
        };

        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(117, 117, 117),
            LineHeight = 1.08f
        };

        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(245, 245, 245); // Light Gray
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(250, 250, 250); // Very Light Gray
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
