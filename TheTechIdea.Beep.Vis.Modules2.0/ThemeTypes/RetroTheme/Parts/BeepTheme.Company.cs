using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Company Colors
        // Note: Ensure 'Courier New' font family is available for retro aesthetic. If unavailable, 'Consolas' is a fallback.
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for popover background
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for title
        public TypographyStyle CompanyTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(255, 215, 0), // Retro yellow
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(192, 192, 192); // Light gray for subtitle
        public TypographyStyle CompanySubTitleFont { get; set; } = new TypographyStyle
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
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(255, 255, 255); // White for description
        public TypographyStyle CompanyDescriptionFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for links
        public TypographyStyle CompanyLinkFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(0, 255, 255), // Bright cyan
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = true,
            IsStrikeout = false
        };
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal for button background
        public Color CompanyButtonTextColor { get; set; } = Color.FromArgb(255, 255, 255); // White for button text
        public TypographyStyle CompanyButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal for dropdown
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(255, 255, 255); // White for dropdown text
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(0, 64, 64); // Mid-tone teal for logo background
    }
}