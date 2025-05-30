using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle NavigationTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle NavigationSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle NavigationUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color NavigationBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for navigation background
        public Color NavigationForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for navigation text
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected
        public Color NavigationSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected text
    }
}