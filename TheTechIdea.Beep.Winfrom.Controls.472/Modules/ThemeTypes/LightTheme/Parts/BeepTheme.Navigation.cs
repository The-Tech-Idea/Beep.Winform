using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle  NavigationTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  NavigationSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  NavigationUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.White;
        public Color NavigationForeColor { get; set; } = Color.Black;
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color NavigationHoverForeColor { get; set; } = Color.DodgerBlue;
        public Color NavigationSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
