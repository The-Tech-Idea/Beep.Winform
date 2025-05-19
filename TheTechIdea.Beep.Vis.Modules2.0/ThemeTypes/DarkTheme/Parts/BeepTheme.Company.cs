using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Company Colors - Dark Theme Adaptation
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(30, 30, 30);   // Dark popover background
        public Color CompanyTitleColor { get; set; } = Color.White;                             // White title text
        public TypographyStyle CompanyTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.LightSteelBlue;                 // Soft blue subtitle
        public TypographyStyle CompanySubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
        public Color CompanyDescriptionColor { get; set; } = Color.LightGray;                   // Light gray description
        public TypographyStyle CompanyDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public Color CompanyLinkColor { get; set; } = Color.Cyan;                               // Cyan links for contrast
        public TypographyStyle CompanyLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Underline);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.Cyan;                   // Bright button background
        public Color CompanyButtonTextColor { get; set; } = Color.Black;                        // Dark text on buttons
        public TypographyStyle CompanyButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(45, 45, 45); // Dark dropdown background
        public Color CompanyDropdownTextColor { get; set; } = Color.White;                      // White dropdown text
        public Color CompanyLogoBackgroundColor { get; set; } = Color.DimGray;                  // Dark gray logo background
    }
}
