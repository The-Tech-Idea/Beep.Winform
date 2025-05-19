using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle NavigationTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle NavigationSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
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
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color NavigationBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for navigation background
        public Color NavigationForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for navigation text
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected
        public Color NavigationSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected text
    }
}