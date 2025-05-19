using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
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

        public Color NavigationBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color NavigationForeColor { get; set; } = Color.Black;
        public Color NavigationHoverBackColor { get; set; } = Color.LightBlue;
        public Color NavigationHoverForeColor { get; set; } = Color.Black;
        public Color NavigationSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
