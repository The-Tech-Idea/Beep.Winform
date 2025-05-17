using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Dashboard Colors & Fonts

        public Font DashboardTitleFont { get; set; } = new Font("Comic Sans MS", 14f, FontStyle.Bold);
        public Font DashboardSubTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Italic);

        public Color DashboardBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint

        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color DashboardTitleBackColor { get; set; } = Color.FromArgb(228, 222, 255); // Pastel Lavender

        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(240, 100, 180), // Candy Pink
            LetterSpacing = 0.12f,
            LineHeight = 1.2f
        };

        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint
        public Color DashboardSubTitleBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow

        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(127, 255, 212), // Mint
            LetterSpacing = 0.05f,
            LineHeight = 1.15f
        };

        // Candy gradient: pink (start), mint (middle), lemon (end)
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(255, 253, 194);   // Lemon Yellow
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
