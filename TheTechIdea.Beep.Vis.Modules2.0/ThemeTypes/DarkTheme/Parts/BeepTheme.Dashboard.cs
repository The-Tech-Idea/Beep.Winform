using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Dashboard Colors & Fonts
        public TypographyStyle DashboardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 20F, FontStyle.Bold);
        public TypographyStyle DashboardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Regular);

        public Color DashboardBackColor { get; set; } = Color.FromArgb(24, 24, 24);
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60);

        public Color DashboardTitleForeColor { get; set; } = Color.White;
        public Color DashboardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20,
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

        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(25, 25, 25);
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(35, 35, 35);
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
