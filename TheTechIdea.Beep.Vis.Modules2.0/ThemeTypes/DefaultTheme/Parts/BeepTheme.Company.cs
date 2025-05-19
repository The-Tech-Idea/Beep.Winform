using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.White;
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(33, 33, 33); // Darker black for better modern contrast
        public TypographyStyle CompanyTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(13, 71, 161); // Darker blue
        public TypographyStyle CompanySubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(117, 117, 117); // Medium gray
        public TypographyStyle CompanyDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(33, 150, 243); // Modern blue
        public TypographyStyle CompanyLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Underline);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue button
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle CompanyButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.White;
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(224, 224, 224); // Light gray background
    }
}
