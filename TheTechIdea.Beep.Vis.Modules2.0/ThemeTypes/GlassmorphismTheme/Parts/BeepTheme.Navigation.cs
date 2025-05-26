using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  NavigationTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle  NavigationSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  NavigationUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color NavigationForeColor { get; set; } = Color.Black;
        public Color NavigationHoverBackColor { get; set; } = Color.LightBlue;
        public Color NavigationHoverForeColor { get; set; } = Color.Black;
        public Color NavigationSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
