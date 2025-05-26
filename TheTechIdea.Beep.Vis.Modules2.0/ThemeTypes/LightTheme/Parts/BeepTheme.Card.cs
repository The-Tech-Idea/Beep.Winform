using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Card Colors & Fonts
//<<<<<<< HEAD
        public TypographyStyle  CardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.Black;
        public Color CardBackColor { get; set; } = Color.White;
        public Color CardTitleForeColor { get; set; } = Color.DarkSlateGray;
        public TypographyStyle  CardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Italic);
        public Color CardSubTitleForeColor { get; set; } = Color.Gray;
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextColor = Color.DarkSlateGray
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Black
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.Light,
            TextColor = Color.Gray
        };
        public Color CardrGradiantStartColor { get; set; } = Color.WhiteSmoke;
        public Color CardGradiantEndColor { get; set; } = Color.Gainsboro;
        public Color CardGradiantMiddleColor { get; set; } = Color.LightGray;
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
