using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color LoginTitleColor { get; set; } = Color.White;
        public TypographyStyle LoginTitleFont { get; set; } = new TypographyStyle
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
        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(200, 200, 200);
        public TypographyStyle LoginSubtitleFont { get; set; } = new TypographyStyle
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
        public Color LoginDescriptionColor { get; set; } = Color.FromArgb(200, 200, 200);
        public TypographyStyle LoginDescriptionFont { get; set; } = new TypographyStyle
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
        public Color LoginLinkColor { get; set; } = Color.FromArgb(255, 193, 7);
        public TypographyStyle LoginLinkFont { get; set; } = new TypographyStyle
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
        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle LoginButtonFont { get; set; } = new TypographyStyle
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
        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color LoginDropdownTextColor { get; set; } = Color.White;
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(52, 58, 64);
    }
}