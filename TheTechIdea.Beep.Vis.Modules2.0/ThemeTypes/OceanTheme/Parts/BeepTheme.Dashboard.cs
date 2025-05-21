using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Dashboard Colors & Fonts
        public TypographyStyle DashboardTitleFont { get; set; } = new TypographyStyle() { FontSize = 18, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle DashboardSubTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public Color DashboardBackColor { get; set; } = Color.FromArgb(240, 245, 250);
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color DashboardTitleForeColor { get; set; } = Color.White;
        public Color DashboardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle() { FontSize = 18, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color DashboardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(0, 105, 148);
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}