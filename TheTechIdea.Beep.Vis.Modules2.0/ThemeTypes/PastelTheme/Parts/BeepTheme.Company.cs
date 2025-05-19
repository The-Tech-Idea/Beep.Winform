using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Company Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for popover background
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for title
        public TypographyStyle CompanyTitleFont { get; set; } = new TypographyStyle
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
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(100, 100, 100); // Medium gray for subtitle
        public TypographyStyle CompanySubTitleFont { get; set; } = new TypographyStyle
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
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for description
        public TypographyStyle CompanyDescriptionFont { get; set; } = new TypographyStyle
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
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for links
        public TypographyStyle CompanyLinkFont { get; set; } = new TypographyStyle
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
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for button background
        public Color CompanyButtonTextColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for button text
        public TypographyStyle CompanyButtonFont { get; set; } = new TypographyStyle
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
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for dropdown
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for dropdown text
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for logo background
    }
}