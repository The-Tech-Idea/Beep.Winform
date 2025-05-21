using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public TypographyStyle CompanyTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.4f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public TypographyStyle CompanySubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(139, 69, 19), // SaddleBrown
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public TypographyStyle CompanyDescriptionFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public TypographyStyle CompanyLinkFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(160, 82, 45), // Sienna
            IsUnderlined = true,
            IsStrikeout = false
        };
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle CompanyButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
    }
}