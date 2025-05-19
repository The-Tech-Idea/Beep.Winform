using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
<<<<<<< HEAD
        public Font NavigationTitleFont { get; set; } = new Font("Segoe UI", 16f, FontStyle.Bold);
        public Font NavigationSelectedFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Font NavigationUnSelectedFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Regular);
=======
        public TypographyStyle NavigationTitleFont { get; set; }
        public TypographyStyle NavigationSelectedFont { get; set; }
        public TypographyStyle NavigationUnSelectedFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634

        public Color NavigationBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color NavigationForeColor { get; set; } = Color.WhiteSmoke;
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color NavigationHoverForeColor { get; set; } = Color.CornflowerBlue;
        public Color NavigationSelectedBackColor { get; set; } = Color.CornflowerBlue;
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
