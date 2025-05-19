using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Company Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for popover background
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for title
        public TypographyStyle CompanyTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for subtitle
        public TypographyStyle CompanySubTitleFont { get; set; } = new TypographyStyle
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
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for description
        public TypographyStyle CompanyDescriptionFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(50, 120, 160); // Deep sky blue for links
        public TypographyStyle CompanyLinkFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(50, 120, 160), // Deep sky blue
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = true,
            IsStrikeout = false
        };
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for button background
        public Color CompanyButtonTextColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for button text
        public TypographyStyle CompanyButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for dropdown
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for dropdown text
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for logo background
    }
}