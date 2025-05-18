using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Dashboard Colors & Fonts
        public Font DashboardTitleFont { get; set; } = new Font("Segoe UI", 18, FontStyle.Bold);
        public Font DashboardSubTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Regular);
        public Color DashboardBackColor { get; set; } = Color.FromArgb(250, 250, 255);
        public Color DashboardCardBackColor { get; set; } = Color.White;
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(230, 240, 255);
        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(0, 51, 102);
        public Color DashboardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(0, 51, 102)
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color DashboardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            TextColor = Color.FromArgb(70, 70, 70)
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(230, 240, 255);
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(245, 248, 255);
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
