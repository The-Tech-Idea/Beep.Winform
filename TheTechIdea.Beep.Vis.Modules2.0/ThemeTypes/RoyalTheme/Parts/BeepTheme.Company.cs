using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color CompanyTitleColor { get; set; } = Color.White;
        public TypographyStyle CompanyTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(200, 200, 200);
        public TypographyStyle CompanySubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(200, 200, 200);
        public TypographyStyle CompanyDescriptionFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(255, 193, 7);
        public TypographyStyle CompanyLinkFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 193, 7),
            IsUnderlined = true,
            IsStrikeout = false
        };
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle CompanyButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color CompanyDropdownTextColor { get; set; } = Color.White;
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(52, 58, 64);
    }
}