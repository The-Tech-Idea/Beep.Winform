using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Dashboard Colors & Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle DashboardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
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
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for background
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for cards
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for card hover
        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for title
        public Color DashboardTitleBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for title background
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for subtitle
        public Color DashboardSubTitleBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for subtitle background
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for ocean depth effect
    }
}