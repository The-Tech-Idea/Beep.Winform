using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.White;
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public TypographyStyle  CompanyTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public TypographyStyle  CompanySubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 12f, FontStyle.Regular);
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(158, 158, 158); // Grey 500
        public TypographyStyle  CompanyDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public TypographyStyle  CompanyLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        // Use Bold instead of Medium
        public TypographyStyle  CompanyButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.White;
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(238, 238, 238); // Grey 200
    }
}
