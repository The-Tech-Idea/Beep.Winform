using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public Font NavigationTitleFont { get; set; } = new Font("Segoe UI", 16f, FontStyle.Bold);
        public Font NavigationSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font NavigationUnSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color NavigationForeColor { get; set; } = Color.LightGray;
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color NavigationHoverForeColor { get; set; } = Color.White;
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color NavigationSelectedForeColor { get; set; } = Color.CornflowerBlue;
    }
}
