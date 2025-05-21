using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Dashboard Colors & Fonts
        public TypographyStyle DashboardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 18, FontStyle.Bold);
        public TypographyStyle DashboardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color DashboardBackColor { get; set; } = Color.Black;
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color DashboardTitleForeColor { get; set; } = Color.White;
        public Color DashboardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.Gray;
        public Color DashboardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color DashboardGradiantEndColor { get; set; } = Color.Black;
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(30, 30, 30);
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
