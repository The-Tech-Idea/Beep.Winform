using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Dashboard Colors & Fonts
        public TypographyStyle  DashboardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  DashboardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);

        public Color DashboardBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(240, 245, 250);
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(230, 240, 250);

        public Color DashboardTitleForeColor { get; set; } = Color.Black;
        public Color DashboardTitleBackColor { get; set; } = Color.FromArgb(220, 230, 240);
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Black,
            LineHeight = 1.3f,
            LetterSpacing = 0f
        };

        public Color DashboardSubTitleForeColor { get; set; } = Color.DimGray;
        public Color DashboardSubTitleBackColor { get; set; } = Color.FromArgb(235, 240, 245);
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Italic,
            TextColor = Color.DimGray,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(215, 230, 245);
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
