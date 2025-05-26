using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.White;
//<<<<<<< HEAD
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(33, 33, 33);    // Almost Black
        public TypographyStyle  CompanyTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);

        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(25, 118, 210);   // Blue
        public TypographyStyle  CompanySubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);

        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(97, 97, 97);     // Gray
        public TypographyStyle  CompanyDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Italic);

        public Color CompanyLinkColor { get; set; } = Color.FromArgb(0, 123, 255);    // Bootstrap blue
        public TypographyStyle  CompanyLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Underline);

        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(63, 81, 181);    // Indigo
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle  CompanyButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);

        public Color CompanyDropdownBackgroundColor { get; set; } = Color.White;
        public Color CompanyDropdownTextColor { get; set; } = Color.Black;

        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(158, 158, 158);  // Medium Gray
    }
}
