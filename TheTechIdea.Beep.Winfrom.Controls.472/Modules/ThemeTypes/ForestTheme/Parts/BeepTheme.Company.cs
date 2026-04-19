using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(245, 255, 245); // Light Mint
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(0, 51, 0); // Dark Green
        public TypographyStyle CompanyTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(0, 102, 51); // Medium Dark Green
        public TypographyStyle CompanySubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(85, 107, 47); // Dark Olive Green
        public TypographyStyle CompanyDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(34, 139, 34); // Forest Green
        public TypographyStyle CompanyLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Underline);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(34, 139, 34); // Forest Green
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle CompanyButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.White;
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(34, 139, 34); // Forest Green
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(46, 139, 87); // Sea Green
    }
}
