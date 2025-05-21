using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color LoginTitleColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public TypographyStyle LoginTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 18,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public TypographyStyle LoginSubtitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color LoginDescriptionColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public TypographyStyle LoginDescriptionFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color LoginLinkColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public TypographyStyle LoginLinkFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(65, 65, 145), // Royal blue
            IsUnderlined = true,
            IsStrikeout = false
        };
        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle LoginButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color LoginDropdownTextColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
    }
}