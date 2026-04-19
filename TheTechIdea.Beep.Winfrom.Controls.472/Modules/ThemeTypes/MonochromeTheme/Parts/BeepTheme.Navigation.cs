using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle NavigationTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle NavigationSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle NavigationUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.White;
        public Color NavigationForeColor { get; set; } = Color.Black;
        public Color NavigationHoverBackColor { get; set; } = Color.LightGray;
        public Color NavigationHoverForeColor { get; set; } = Color.Black;
        public Color NavigationSelectedBackColor { get; set; } = Color.DarkGray;
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
