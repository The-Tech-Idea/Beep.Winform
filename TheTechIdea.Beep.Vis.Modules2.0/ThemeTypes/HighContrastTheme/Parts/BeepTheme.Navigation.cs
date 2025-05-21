using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
//<<<<<<< HEAD
        public Font NavigationTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font NavigationSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font NavigationUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.Black;
        public Color NavigationForeColor { get; set; } = Color.White;
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color NavigationHoverForeColor { get; set; } = Color.Yellow;
        public Color NavigationSelectedBackColor { get; set; } = Color.Yellow;
        public Color NavigationSelectedForeColor { get; set; } = Color.Black;
    }
}
