using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Dashboard Colors & Fonts
        public TypographyStyle DashboardTitleFont { get; set; } = new TypographyStyle() { FontSize = 18, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle DashboardSubTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public Color DashboardBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color DashboardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle() { FontSize = 18, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color DashboardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(247, 221, 229);
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(242, 201, 215);
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}