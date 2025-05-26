using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Dashboard Colors & Fonts
//<<<<<<< HEAD
        public TypographyStyle  DashboardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 24f, FontStyle.Bold);
        public TypographyStyle  DashboardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 16f, FontStyle.Regular);
        public Color DashboardBackColor { get; set; } = Color.FromArgb(250, 250, 250);
        public Color DashboardCardBackColor { get; set; } = Color.White;
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color DashboardTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 24f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(33, 33, 33)
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(117, 117, 117);
        public Color DashboardSubTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(117, 117, 117)
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(250, 250, 250);
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
