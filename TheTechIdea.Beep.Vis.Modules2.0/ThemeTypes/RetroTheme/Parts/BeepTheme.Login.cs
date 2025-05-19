using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color LoginTitleColor { get; set; } = Color.White;
        public TypographyStyle LoginTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 16, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(147, 161, 161);
        public TypographyStyle LoginSubtitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(147, 161, 161), IsUnderlined = false, IsStrikeout = false };
        public Color LoginDescriptionColor { get; set; } = Color.FromArgb(108, 123, 127);
        public TypographyStyle LoginDescriptionFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(108, 123, 127), IsUnderlined = false, IsStrikeout = false };
        public Color LoginLinkColor { get; set; } = Color.FromArgb(38, 139, 210);
        public TypographyStyle LoginLinkFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(38, 139, 210), IsUnderlined = true, IsStrikeout = false };
        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color LoginButtonTextColor { get; set; } = Color.Black;
        public TypographyStyle LoginButtonFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color LoginDropdownTextColor { get; set; } = Color.White;
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(7, 54, 66);
    }
}