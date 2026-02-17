using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Company Colors - Desert Theme
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(255, 248, 238); // Soft warm cream
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(92, 60, 22); // Dark earthy brown
        public TypographyStyle CompanyTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(157, 113, 51); // Warm tan
        public TypographyStyle CompanySubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(135, 108, 78); // Muted brownish gray
        public TypographyStyle CompanyDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(178, 117, 61); // Rustic orange/brown
        public TypographyStyle CompanyLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Underline);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(191, 140, 75); // Soft desert gold
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle CompanyButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(255, 248, 238); // same as popover bg
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(92, 60, 22); // dark brown text
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(210, 180, 140); // Light tan desert sand
    }
}
