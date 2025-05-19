using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Dashboard Colors & Fonts
        // Note: Ensure 'Courier New' font family is available for retro aesthetic. If unavailable, 'Consolas' is a fallback.
        public TypographyStyle DashboardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(255, 215, 0), // Retro yellow
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DashboardSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 14f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(192, 192, 192), // Light gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardBackColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for background
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(0, 64, 64); // Mid-tone teal for cards
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(0, 170, 170); // Lighter teal for card hover
        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for title
        public Color DashboardTitleBackColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for title background
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(255, 215, 0), // Retro yellow
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(192, 192, 192); // Light gray for subtitle
        public Color DashboardSubTitleBackColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for subtitle background
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 14f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(192, 192, 192), // Light gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(0, 43, 43); // Darker teal
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(0, 64, 64); // Mid-tone teal
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for retro CRT effect
    }
}