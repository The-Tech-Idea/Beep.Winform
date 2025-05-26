using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.White;
        public Color LoginTitleColor { get; set; } = Color.Black;
//<<<<<<< HEAD
        public TypographyStyle  LoginTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16F, FontStyle.Bold);
        public Color LoginSubtitleColor { get; set; } = Color.DarkBlue;
        public TypographyStyle  LoginSubtitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
        public Color LoginDescriptionColor { get; set; } = Color.Gray;
        public TypographyStyle  LoginDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public Color LoginLinkColor { get; set; } = Color.Blue;
        public TypographyStyle  LoginLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Underline);
        public Color LoginButtonBackgroundColor { get; set; } = Color.Blue;
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle  LoginButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
        public Color LoginDropdownBackgroundColor { get; set; } = Color.White;
        public Color LoginDropdownTextColor { get; set; } = Color.Black;
        public Color LoginLogoBackgroundColor { get; set; } = Color.LightGray;
    }
}
