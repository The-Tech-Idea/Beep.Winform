using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color LoginTitleColor { get; set; } = Color.White;
        public TypographyStyle  LoginTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16f, FontStyle.Bold);

        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(0xA0, 0xA0, 0xFF); // Soft violet
        public TypographyStyle  LoginSubtitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);

        public Color LoginDescriptionColor { get; set; } = Color.LightGray;
        public TypographyStyle  LoginDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Italic);

        public Color LoginLinkColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Sky blue
        public TypographyStyle  LoginLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Underline);

        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle  LoginButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);

        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(0x16, 0x21, 0x3E); // SecondaryColor
        public Color LoginDropdownTextColor { get; set; } = Color.White;
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Deep galaxy blue
    }
}
