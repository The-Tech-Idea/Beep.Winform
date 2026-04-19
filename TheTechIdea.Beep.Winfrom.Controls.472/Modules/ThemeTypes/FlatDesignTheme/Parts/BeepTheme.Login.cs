using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.White;
        public Color LoginTitleColor { get; set; } = Color.FromArgb(33, 33, 33);
        public TypographyStyle LoginTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(0, 120, 215);
        public TypographyStyle LoginSubtitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);
        public Color LoginDescriptionColor { get; set; } = Color.Gray;
        public TypographyStyle LoginDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color LoginLinkColor { get; set; } = Color.FromArgb(0, 120, 215);
        public TypographyStyle LoginLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Underline);
        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle LoginButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color LoginDropdownBackgroundColor { get; set; } = Color.White;
        public Color LoginDropdownTextColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color LoginLogoBackgroundColor { get; set; } = Color.LightGray;
    }
}
