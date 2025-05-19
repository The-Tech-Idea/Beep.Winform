using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Dashboard Colors & Fonts
<<<<<<< HEAD
        public Font DashboardTitleFont { get; set; } = new Font("Segoe UI", 16f, FontStyle.Bold);
        public Font DashboardSubTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Italic);

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
