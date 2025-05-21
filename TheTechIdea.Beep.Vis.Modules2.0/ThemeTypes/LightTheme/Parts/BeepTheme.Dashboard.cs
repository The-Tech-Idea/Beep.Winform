using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Dashboard Colors & Fonts
//<<<<<<< HEAD
        public Font DashboardTitleFont { get; set; } = new Font("Segoe UI", 18F, FontStyle.Bold);
        public Font DashboardSubTitleFont { get; set; } = new Font("Segoe UI", 14F, FontStyle.Regular);
        public Color DashboardBackColor { get; set; } = Color.WhiteSmoke;
        public Color DashboardCardBackColor { get; set; } = Color.White;
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(230, 240, 255);
        public Color DashboardTitleForeColor { get; set; } = Color.Black;
        public Color DashboardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Black
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.DimGray;
        public Color DashboardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.DimGray
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.White;
        public Color DashboardGradiantEndColor { get; set; } = Color.LightGray;
        public Color DashboardGradiantMiddleColor { get; set; } = Color.Gainsboro;
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
