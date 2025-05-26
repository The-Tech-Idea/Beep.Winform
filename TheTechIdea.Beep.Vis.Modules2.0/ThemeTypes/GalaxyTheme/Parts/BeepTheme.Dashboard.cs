using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Dashboard Colors & Fonts
//<<<<<<< HEAD
        public TypographyStyle  DashboardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 18f, FontStyle.Bold);
        public TypographyStyle  DashboardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Regular);

        public Color DashboardBackColor { get; set; } = Color.FromArgb(0x05, 0x05, 0x14); // BackgroundColor
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor

        public Color DashboardTitleForeColor { get; set; } = Color.White;
        public Color DashboardTitleBackColor { get; set; } =Color.FromArgb(10, 10, 30);
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.SemiBold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.3f,
            LetterSpacing = 0
        };

        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(0xC0, 0xC0, 0xFF); // Light bluish
        public Color DashboardSubTitleBackColor { get; set; } =Color.FromArgb(10, 10, 30);
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(0xC0, 0xC0, 0xFF),
            LineHeight = 1.2f,
            LetterSpacing = 0
        };

        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(0x1A, 0x1A, 0x2E); // PrimaryColor
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(0x16, 0x21, 0x3E); // SecondaryColor
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
