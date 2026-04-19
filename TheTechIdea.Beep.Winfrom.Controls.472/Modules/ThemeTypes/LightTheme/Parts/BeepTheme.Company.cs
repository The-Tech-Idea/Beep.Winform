using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.White;
        public Color CompanyTitleColor { get; set; } = Color.Black;
        public TypographyStyle  CompanyTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.DarkBlue;
        public TypographyStyle  CompanySubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);
        public Color CompanyDescriptionColor { get; set; } = Color.Gray;
        public TypographyStyle  CompanyDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color CompanyLinkColor { get; set; } = Color.DodgerBlue;
        public TypographyStyle  CompanyLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Underline);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.RoyalBlue;
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle  CompanyButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.White;
        public Color CompanyDropdownTextColor { get; set; } = Color.Black;
        public Color CompanyLogoBackgroundColor { get; set; } = Color.LightGray;
    }
}
