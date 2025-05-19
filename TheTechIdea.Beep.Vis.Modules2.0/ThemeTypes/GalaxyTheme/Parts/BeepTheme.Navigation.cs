using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
<<<<<<< HEAD
        public Font NavigationTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font NavigationSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font NavigationUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
=======
        public TypographyStyle NavigationTitleFont { get; set; }
        public TypographyStyle NavigationSelectedFont { get; set; }
        public TypographyStyle NavigationUnSelectedFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634

        public Color NavigationBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color NavigationForeColor { get; set; } = Color.White;
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Hover shade
        public Color NavigationHoverForeColor { get; set; } = Color.White;
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
