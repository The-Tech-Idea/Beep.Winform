using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Dashboard Colors & Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle DashboardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for background
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(220, 220, 225); // Slightly darker gray for cards
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Darker gray for card hover
        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for title
        public Color DashboardTitleBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for title background
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for subtitle
        public Color DashboardSubTitleBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for subtitle background
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(240, 240, 245); // Light gray gradient start
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(210, 210, 215); // Darker gray gradient end
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(230, 230, 235); // Mid-tone gray for gradient
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for soft neumorphic effect
    }
}