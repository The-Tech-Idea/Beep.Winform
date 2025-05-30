using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Company Colors
//<<<<<<< HEAD
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(30, 30, 30); // Dark background
        public Color CompanyTitleColor { get; set; } = Color.WhiteSmoke; // Light text
        public TypographyStyle  CompanyTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.LightSteelBlue;
        public TypographyStyle  CompanySubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);
        public Color CompanyDescriptionColor { get; set; } = Color.Gray;
        public TypographyStyle  CompanyDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Italic);
        public Color CompanyLinkColor { get; set; } = Color.CornflowerBlue;
        public TypographyStyle  CompanyLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Underline);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(48, 63, 159); // Indigo 700
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle  CompanyButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color CompanyDropdownTextColor { get; set; } = Color.WhiteSmoke;
        public Color CompanyLogoBackgroundColor { get; set; } = Color.DimGray;
    }
}
