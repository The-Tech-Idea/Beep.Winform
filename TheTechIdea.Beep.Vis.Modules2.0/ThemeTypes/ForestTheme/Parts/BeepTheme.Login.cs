using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(232, 245, 233); // Light forest green
        public Color LoginTitleColor { get; set; } = Color.FromArgb(27, 94, 32); // Dark forest green
        public TypographyStyle LoginTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16F, FontStyle.Bold);
        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(56, 142, 60); // Medium forest green
        public TypographyStyle LoginSubtitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
        public Color LoginDescriptionColor { get; set; } = Color.FromArgb(85, 107, 47); // Olive green
        public TypographyStyle LoginDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Italic);
        public Color LoginLinkColor { get; set; } = Color.FromArgb(46, 125, 50); // Green link
        public TypographyStyle LoginLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Underline);
        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(56, 142, 60); // Button green
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle LoginButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(232, 245, 233);
        public Color LoginDropdownTextColor { get; set; } = Color.FromArgb(27, 94, 32);
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(197, 225, 165); // Soft green background
    }
}
