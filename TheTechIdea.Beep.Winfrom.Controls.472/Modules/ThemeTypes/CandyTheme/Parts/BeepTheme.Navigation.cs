using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors

        public TypographyStyle NavigationTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 14f, FontStyle.Bold);
        public TypographyStyle NavigationSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 8f, FontStyle.Bold);
        public TypographyStyle NavigationUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        // Default navigation background/foreground
        public Color NavigationBackColor { get; set; } = Color.FromArgb(255, 224, 235);      // Pastel Pink
        public Color NavigationForeColor { get; set; } = Color.FromArgb(44, 62, 80);         // Navy

        // Hover state: pastel blue with candy pink text
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        // Selected state: mint background, lemon text
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
        public Color NavigationSelectedForeColor { get; set; } = Color.FromArgb(255, 223, 93);  // Lemon Yellow
    }
}
