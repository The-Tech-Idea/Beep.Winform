using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(25, 25, 25);
        public Color LoginTitleColor { get; set; } = Color.White;
        public TypographyStyle  LoginTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 18f, FontStyle.Bold);
        public Color LoginSubtitleColor { get; set; } = Color.LightGray;
        public TypographyStyle  LoginSubtitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Regular);
        public Color LoginDescriptionColor { get; set; } = Color.Gray;
        public TypographyStyle  LoginDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);
        public Color LoginLinkColor { get; set; } = Color.DodgerBlue;
        public TypographyStyle  LoginLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Underline);
        public Color LoginButtonBackgroundColor { get; set; } = Color.DodgerBlue;
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle  LoginButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(35, 35, 35);
        public Color LoginDropdownTextColor { get; set; } = Color.White;
        public Color LoginLogoBackgroundColor { get; set; } = Color.DarkGray;
    }
}
