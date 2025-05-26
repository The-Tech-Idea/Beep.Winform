using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  NavigationTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16f, FontStyle.Bold);
        public TypographyStyle  NavigationSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  NavigationUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color NavigationForeColor { get; set; } = Color.WhiteSmoke;
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color NavigationHoverForeColor { get; set; } = Color.CornflowerBlue;
        public Color NavigationSelectedBackColor { get; set; } = Color.CornflowerBlue;
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
