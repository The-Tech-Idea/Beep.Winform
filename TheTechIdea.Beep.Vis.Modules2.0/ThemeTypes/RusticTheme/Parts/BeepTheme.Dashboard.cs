using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Dashboard Colors & Fonts
        public TypographyStyle DashboardTitleFont { get; set; }
        public TypographyStyle DashboardSubTitleFont { get; set; }
        public Color DashboardBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color DashboardTitleBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public TypographyStyle DashboardTitleStyle { get; set; }
        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color DashboardSubTitleBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public TypographyStyle DashboardSubTitleStyle { get; set; }
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(210, 180, 140);
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(238, 232, 205);
        public LinearGradientMode DashboardGradiantDirection { get; set; }
    }
}
