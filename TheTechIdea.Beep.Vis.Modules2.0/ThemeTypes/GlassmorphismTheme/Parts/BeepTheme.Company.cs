using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color CompanyTitleColor { get; set; } = Color.Black;
        public TypographyStyle  CompanyTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);

        public Color CompanySubtitleColor { get; set; } = Color.DarkBlue;
        public TypographyStyle  CompanySubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);

        public Color CompanyDescriptionColor { get; set; } = Color.Gray;
        public TypographyStyle  CompanyDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);

        public Color CompanyLinkColor { get; set; } = Color.SteelBlue;
        public TypographyStyle  CompanyLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Underline);

        public Color CompanyButtonBackgroundColor { get; set; } = Color.SkyBlue;
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle  CompanyButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);

        public Color CompanyDropdownBackgroundColor { get; set; } = Color.WhiteSmoke;
        public Color CompanyDropdownTextColor { get; set; } = Color.Black;

        public Color CompanyLogoBackgroundColor { get; set; } = Color.LightGray;
    }
}
