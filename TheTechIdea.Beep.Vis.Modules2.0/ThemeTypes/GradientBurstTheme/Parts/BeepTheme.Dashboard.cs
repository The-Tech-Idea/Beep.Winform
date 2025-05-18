using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Dashboard Colors & Fonts
        public Font DashboardTitleFont { get; set; } = new Font("Segoe UI", 18f, FontStyle.Bold);
        public Font DashboardSubTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Regular);

        public Color DashboardBackColor { get; set; } = Color.FromArgb(250, 250, 250); // Light Gray
        public Color DashboardCardBackColor { get; set; } = Color.White;
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(245, 245, 255); // Soft Blue Tint

        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(33, 33, 33);    // Deep Gray
        public Color DashboardTitleBackColor { get; set; } = Color.Transparent;             // Use panel color

        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(66, 66, 66);
        public Color DashboardSubTitleBackColor { get; set; } = Color.Transparent;

        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(66, 66, 66)
        };

        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(63, 81, 181);   // Indigo
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(255, 87, 34);   // Deep Orange
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(103, 58, 183);  // Deep Purple
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
