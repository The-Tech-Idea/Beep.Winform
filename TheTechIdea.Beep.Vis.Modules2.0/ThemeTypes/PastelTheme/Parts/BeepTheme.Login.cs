using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Login Popover Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for popover background
        public Color LoginTitleColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for title
        public TypographyStyle LoginTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(100, 100, 100); // Medium gray for subtitle
        public TypographyStyle LoginSubtitleFont { get; set; } = new TypographyStyle
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
        public Color LoginDescriptionColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for description
        public TypographyStyle LoginDescriptionFont { get; set; } = new TypographyStyle
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
        public Color LoginLinkColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for links
        public TypographyStyle LoginLinkFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = true,
            IsStrikeout = false
        };
        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for button background
        public Color LoginButtonTextColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for button text
        public TypographyStyle LoginButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for dropdown
        public Color LoginDropdownTextColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for dropdown text
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for logo background
    }
}