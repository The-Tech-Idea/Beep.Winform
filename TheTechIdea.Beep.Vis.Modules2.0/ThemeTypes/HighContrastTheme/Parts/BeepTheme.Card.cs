using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Card Colors & Fonts
//<<<<<<< HEAD
        public TypographyStyle  CardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.White;
        public Color CardBackColor { get; set; } = Color.Black;
        public Color CardTitleForeColor { get; set; } = Color.Yellow;
        public TypographyStyle  CardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color CardSubTitleForeColor { get; set; } = Color.LightGray;
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Bold,
            TextColor = Color.Yellow
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightGray
        };
        public Color CardrGradiantStartColor { get; set; } = Color.Black;
        public Color CardGradiantEndColor { get; set; } = Color.DarkSlateGray;
        public Color CardGradiantMiddleColor { get; set; } = Color.Gray;
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
