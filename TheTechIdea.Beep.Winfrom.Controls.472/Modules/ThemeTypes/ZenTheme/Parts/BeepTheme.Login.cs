using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color LoginTitleColor { get; set; } = Color.FromArgb(34, 34, 34);
        public TypographyStyle LoginTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 18,
            LineHeight = 1.3f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(34, 34, 34),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(64, 64, 64);
        public TypographyStyle LoginSubtitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(64, 64, 64),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color LoginDescriptionColor { get; set; } = Color.FromArgb(189, 189, 189);
        public TypographyStyle LoginDescriptionFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.5f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color LoginLinkColor { get; set; } = Color.FromArgb(76, 175, 80);
        public TypographyStyle LoginLinkFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(76, 175, 80),
            IsUnderlined = true,
            IsStrikeout = false
        };
        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle LoginButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color LoginDropdownTextColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(189, 189, 189);
    }
}