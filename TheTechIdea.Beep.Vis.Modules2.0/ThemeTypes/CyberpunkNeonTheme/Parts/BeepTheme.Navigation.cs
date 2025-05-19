using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors

        public TypographyStyle NavigationTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 13f, FontStyle.Bold);
        public TypographyStyle NavigationSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle NavigationUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.FromArgb(18, 18, 32);              // Cyberpunk Black
        public Color NavigationForeColor { get; set; } = Color.FromArgb(0, 255, 255);             // Neon Cyan

        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(0, 255, 128);        // Neon Green
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);        // Neon Yellow

        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(255, 0, 255);     // Neon Magenta
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
