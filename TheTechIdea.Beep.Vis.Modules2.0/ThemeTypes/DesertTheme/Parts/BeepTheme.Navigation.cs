using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle NavigationTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16, FontStyle.Bold);
        public TypographyStyle NavigationSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle NavigationUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.FromArgb(245, 230, 210); // light sand
        public Color NavigationForeColor { get; set; } = Color.FromArgb(101, 67, 33); // dark brown
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(233, 196, 106); // warm yellow
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(60, 42, 20); // deep brown
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(210, 105, 30); // chocolate brown
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
