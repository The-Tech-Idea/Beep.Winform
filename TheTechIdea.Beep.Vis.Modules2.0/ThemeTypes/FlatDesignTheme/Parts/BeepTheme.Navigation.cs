using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle NavigationTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16, FontStyle.Bold);
        public TypographyStyle NavigationSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle NavigationUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.White;
        public Color NavigationForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
