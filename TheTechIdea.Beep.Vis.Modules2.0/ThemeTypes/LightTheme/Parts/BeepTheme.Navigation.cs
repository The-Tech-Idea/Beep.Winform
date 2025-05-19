using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
<<<<<<< HEAD
        public Font NavigationTitleFont { get; set; } = new Font("Segoe UI", 16f, FontStyle.Bold);
        public Font NavigationSelectedFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Regular);
        public Font NavigationUnSelectedFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Regular);
=======
        public TypographyStyle NavigationTitleFont { get; set; }
        public TypographyStyle NavigationSelectedFont { get; set; }
        public TypographyStyle NavigationUnSelectedFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634

        public Color NavigationBackColor { get; set; } = Color.White;
        public Color NavigationForeColor { get; set; } = Color.Black;
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color NavigationHoverForeColor { get; set; } = Color.DodgerBlue;
        public Color NavigationSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
