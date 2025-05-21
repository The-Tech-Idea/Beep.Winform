using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color LoginTitleColor { get; set; } = Color.FromArgb(80, 80, 80);
        public TypographyStyle LoginTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(120, 120, 120);
        public TypographyStyle LoginSubtitleFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public Color LoginDescriptionColor { get; set; } = Color.FromArgb(150, 150, 150);
        public TypographyStyle LoginDescriptionFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(150, 150, 150) };
        public Color LoginLinkColor { get; set; } = Color.FromArgb(245, 183, 203);
        public TypographyStyle LoginLinkFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(245, 183, 203), IsUnderlined = true };
        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle LoginButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color LoginDropdownTextColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(242, 201, 215);
    }
}