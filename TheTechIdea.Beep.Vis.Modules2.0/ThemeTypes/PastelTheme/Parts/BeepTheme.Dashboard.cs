using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Dashboard Colors & Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle DashboardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DashboardSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for background
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(255, 255, 255); // White for cards
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for card hover
        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for title
        public Color DashboardTitleBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for title background
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(100, 100, 100); // Medium gray for subtitle
        public Color DashboardSubTitleBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for subtitle background
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(220, 215, 230); // Pastel lavender
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for soft pastel effect
    }
}