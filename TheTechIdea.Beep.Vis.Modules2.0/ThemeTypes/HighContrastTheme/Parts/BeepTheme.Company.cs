using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.Black;
        public Color CompanyTitleColor { get; set; } = Color.White;
        public TypographyStyle  CompanyTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.Yellow;
        public TypographyStyle  CompanySubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public Color CompanyDescriptionColor { get; set; } = Color.LightGray;
        public TypographyStyle  CompanyDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color CompanyLinkColor { get; set; } = Color.Cyan;
        public TypographyStyle  CompanyLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Underline);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.Yellow;
        public Color CompanyButtonTextColor { get; set; } = Color.Black;
        public TypographyStyle  CompanyButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.Black;
        public Color CompanyDropdownTextColor { get; set; } = Color.White;
        public Color CompanyLogoBackgroundColor { get; set; } = Color.White;
    }
}
