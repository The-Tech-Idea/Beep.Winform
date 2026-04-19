using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle  NavigationTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  NavigationSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  NavigationUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color NavigationForeColor { get; set; } = Color.White;
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Hover shade
        public Color NavigationHoverForeColor { get; set; } = Color.White;
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
