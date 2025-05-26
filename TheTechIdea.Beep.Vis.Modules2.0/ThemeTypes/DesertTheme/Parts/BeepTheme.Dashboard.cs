using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Dashboard Colors & Fonts

        public TypographyStyle DashboardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI Semibold", 20, FontStyle.Bold);
        public TypographyStyle DashboardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Regular);

        public Color DashboardBackColor { get; set; } = Color.FromArgb(255, 244, 229); // Soft beige
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(245, 222, 179); // Wheat tone
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(238, 214, 175); // Pale gold highlight on hover

        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(101, 67, 33); // Rich brown
        public Color DashboardTitleBackColor { get; set; } =Color.FromArgb(210, 180, 140);

        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI Semibold",
            FontSize = 20,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Bold,
            TextColor = Color.FromArgb(101, 67, 33),
            LineHeight = 1.2f
        };

        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(160, 110, 50); // Darker tan
        public Color DashboardSubTitleBackColor { get; set; } =Color.FromArgb(210, 180, 140);

        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(160, 110, 50),
            LineHeight = 1.1f
        };

        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(255, 248, 220); // Creamy light start
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan sand end
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(238, 214, 175); // Pale gold mid

        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
