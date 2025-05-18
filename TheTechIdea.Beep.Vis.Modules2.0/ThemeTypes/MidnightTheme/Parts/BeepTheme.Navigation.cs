using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public Font NavigationTitleFont { get; set; } = new Font("Segoe UI", 16f, FontStyle.Bold);
        public Font NavigationSelectedFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Font NavigationUnSelectedFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color NavigationForeColor { get; set; } = Color.WhiteSmoke;
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color NavigationHoverForeColor { get; set; } = Color.CornflowerBlue;
        public Color NavigationSelectedBackColor { get; set; } = Color.CornflowerBlue;
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
