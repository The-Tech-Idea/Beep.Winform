using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Dashboard Colors & Fonts
<<<<<<< HEAD
        public Font DashboardTitleFont { get; set; } = new Font("Segoe UI", 18f, FontStyle.Bold);
        public Font DashboardSubTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Regular);

        public Color DashboardBackColor { get; set; } = Color.FromArgb(0x05, 0x05, 0x14); // BackgroundColor
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor

        public Color DashboardTitleForeColor { get; set; } = Color.White;
        public Color DashboardTitleBackColor { get; set; } = Color.Transparent;
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
        public Color DashboardSubTitleBackColor { get; set; } = Color.Transparent;
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
=======
        public TypographyStyle DashboardTitleFont { get; set; }
        public TypographyStyle DashboardSubTitleFont { get; set; }
        public Color DashboardBackColor { get; set; }
        public Color DashboardCardBackColor { get; set; }
        public Color DashboardCardHoverBackColor { get; set; }
        public Color DashboardTitleForeColor { get; set; }
        public Color DashboardTitleBackColor { get; set; }
        public TypographyStyle DashboardTitleStyle { get; set; }
        public Color DashboardSubTitleForeColor { get; set; }
        public Color DashboardSubTitleBackColor { get; set; }
        public TypographyStyle DashboardSubTitleStyle { get; set; }
        public Color DashboardGradiantStartColor { get; set; }
        public Color DashboardGradiantEndColor { get; set; }
        public Color DashboardGradiantMiddleColor { get; set; }
        public LinearGradientMode DashboardGradiantDirection { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
