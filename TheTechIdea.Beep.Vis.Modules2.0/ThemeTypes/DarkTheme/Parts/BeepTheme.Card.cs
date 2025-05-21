using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Card Colors & Fonts
        public TypographyStyle CardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.LightGray;
        public Color CardBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color CardTitleForeColor { get; set; } = Color.White;
        public TypographyStyle CardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public Color CardSubTitleForeColor { get; set; } = Color.Gray;
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontStyle = FontStyle.Bold,
            TextColor = Color.White
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightGray
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontStyle = FontStyle.Italic,
            TextColor = Color.Gray
        };
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(45, 45, 48);
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(38, 38, 38);
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
