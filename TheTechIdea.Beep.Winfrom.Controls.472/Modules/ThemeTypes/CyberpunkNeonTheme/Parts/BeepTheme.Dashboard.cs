using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Dashboard Colors & Fonts

        public TypographyStyle DashboardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14f, FontStyle.Bold);
        public TypographyStyle DashboardSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Italic);

        public Color DashboardBackColor { get; set; } = Color.FromArgb(18, 18, 32);              // Cyberpunk Black
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(34, 34, 68);          // Cyberpunk Panel
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(0, 255, 255);    // Neon Cyan

        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(255, 0, 255);        // Neon Magenta
        public Color DashboardTitleBackColor { get; set; } = Color.FromArgb(36, 0, 70);          // Dark Magenta

        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 0, 255), // Neon Magenta
            LetterSpacing = 1.4f,
            LineHeight = 1.12f
        };

        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(0, 255, 128);     // Neon Green
        public Color DashboardSubTitleBackColor { get; set; } = Color.FromArgb(24, 24, 48);      // Match main background

        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(0, 255, 128), // Neon Green
            LetterSpacing = 1.1f,
            LineHeight = 1.1f
        };

        // Neon gradient for dashboard highlights (e.g., chart cards, KPI panels, etc.)
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(255, 0, 255);   // Neon Magenta
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(0, 255, 255);     // Neon Cyan
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(255, 255, 0);  // Neon Yellow
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}
