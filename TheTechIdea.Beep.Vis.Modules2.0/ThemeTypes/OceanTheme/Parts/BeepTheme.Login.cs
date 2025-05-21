using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(240, 245, 250);
        public Color LoginTitleColor { get; set; } = Color.FromArgb(0, 80, 120);
        public TypographyStyle LoginTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120) };
        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(0, 105, 148);
        public TypographyStyle LoginSubtitleFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(0, 105, 148) };
        public Color LoginDescriptionColor { get; set; } = Color.FromArgb(0, 130, 180);
        public TypographyStyle LoginDescriptionFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 130, 180) };
        public Color LoginLinkColor { get; set; } = Color.FromArgb(0, 150, 200);
        public TypographyStyle LoginLinkFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 150, 200), IsUnderlined = true };
        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle LoginButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color LoginDropdownTextColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(0, 105, 148);
    }
}