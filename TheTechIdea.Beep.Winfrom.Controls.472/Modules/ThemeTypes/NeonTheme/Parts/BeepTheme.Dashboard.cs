using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Dashboard Colors & Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle DashboardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.4f,
            LetterSpacing = 0.5f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DashboardSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(46, 204, 113), // Neon green
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for background
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for cards
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover
        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for title
        public Color DashboardTitleBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for title background
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.4f,
            LetterSpacing = 0.5f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for subtitle
        public Color DashboardSubTitleBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for subtitle background
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(46, 204, 113), // Neon green
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for sleek flow
    }
}