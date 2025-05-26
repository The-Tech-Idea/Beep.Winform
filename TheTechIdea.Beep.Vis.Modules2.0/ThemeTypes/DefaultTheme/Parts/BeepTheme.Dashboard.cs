using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Dashboard Colors & Fonts
        public TypographyStyle DashboardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 20, FontStyle.Bold);
        public TypographyStyle DashboardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Regular);
        public Color DashboardBackColor { get; set; } = Color.White;
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color DashboardTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(33, 33, 33),
            LineHeight = 1.2f
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(100, 100, 100);
        public Color DashboardSubTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(100, 100, 100),
            LineHeight = 1.1f
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(240, 240, 240);
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
