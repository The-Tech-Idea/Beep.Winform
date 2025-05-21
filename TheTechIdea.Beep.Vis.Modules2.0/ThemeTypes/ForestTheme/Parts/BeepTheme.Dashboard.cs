using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Dashboard Colors & Fonts
        public TypographyStyle DashboardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 18, FontStyle.Bold);
        public TypographyStyle DashboardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Regular);
        public Color DashboardBackColor { get; set; } = Color.FromArgb(40, 55, 40); // Dark green background
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(60, 85, 60); // Slightly lighter card background
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(80, 110, 80); // Hover effect card background
        public Color DashboardTitleForeColor { get; set; } = Color.White;
        public Color DashboardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.LightGray;
        public Color DashboardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(50, 70, 50);
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(30, 45, 30);
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(40, 60, 40);
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
