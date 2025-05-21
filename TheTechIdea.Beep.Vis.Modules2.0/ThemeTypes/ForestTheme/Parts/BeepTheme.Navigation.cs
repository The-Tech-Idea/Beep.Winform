using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle NavigationTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI Semibold", 16, FontStyle.Bold);
        public TypographyStyle NavigationSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle NavigationUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.FromArgb(34, 49, 34); // dark forest green
        public Color NavigationForeColor { get; set; } = Color.FromArgb(200, 230, 200); // light mossy green
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(46, 71, 46); // slightly lighter green
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(220, 240, 220); // near white green
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(76, 175, 80); // accent green
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
