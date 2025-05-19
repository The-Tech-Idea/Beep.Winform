using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
<<<<<<< HEAD
        public Font NavigationTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font NavigationSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font NavigationUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
=======
        public TypographyStyle NavigationTitleFont { get; set; }
        public TypographyStyle NavigationSelectedFont { get; set; }
        public TypographyStyle NavigationUnSelectedFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634

        public Color NavigationBackColor { get; set; } = Color.FromArgb(245, 248, 255);
        public Color NavigationForeColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(225, 240, 255);
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(0, 120, 212);
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 212);
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
