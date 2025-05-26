using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Login Popover Colors
//<<<<<<< HEAD
        public Color LoginPopoverBackgroundColor { get; set; } = Color.Black;
        public Color LoginTitleColor { get; set; } = Color.White;
        public TypographyStyle  LoginTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16, FontStyle.Bold);
        public Color LoginSubtitleColor { get; set; } = Color.LightGray;
        public TypographyStyle  LoginSubtitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Italic);
        public Color LoginDescriptionColor { get; set; } = Color.Gray;
        public TypographyStyle  LoginDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color LoginLinkColor { get; set; } = Color.Yellow;
        public TypographyStyle  LoginLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Underline);
        public Color LoginButtonBackgroundColor { get; set; } = Color.White;
        public Color LoginButtonTextColor { get; set; } = Color.Black;
        public TypographyStyle  LoginButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public Color LoginDropdownBackgroundColor { get; set; } = Color.Black;
        public Color LoginDropdownTextColor { get; set; } = Color.White;
        public Color LoginLogoBackgroundColor { get; set; } = Color.DarkGray;
    }
}
