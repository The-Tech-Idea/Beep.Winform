using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle NavigationTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16f, FontStyle.Bold);
        public TypographyStyle NavigationSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle NavigationUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color NavigationForeColor { get; set; } = Color.LightGray;
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color NavigationHoverForeColor { get; set; } = Color.White;
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color NavigationSelectedForeColor { get; set; } = Color.CornflowerBlue;
    }
}
