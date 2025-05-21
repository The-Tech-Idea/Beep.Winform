using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Dashboard Colors & Fonts
//<<<<<<< HEAD
        public Font DashboardTitleFont { get; set; } = new Font("Segoe UI", 18, FontStyle.Bold);
        public Font DashboardSubTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Regular);

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
            FontStyle = FontStyle.Regular,
            TextColor = Color.White
        };

        public Color DashboardSubTitleForeColor { get; set; } = Color.LightGray;
        public Color DashboardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightGray
        };

        public Color DashboardGradiantStartColor { get; set; } = Color.Black;
        public Color DashboardGradiantEndColor { get; set; } = Color.DimGray;
        public Color DashboardGradiantMiddleColor { get; set; } = Color.Gray;
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
