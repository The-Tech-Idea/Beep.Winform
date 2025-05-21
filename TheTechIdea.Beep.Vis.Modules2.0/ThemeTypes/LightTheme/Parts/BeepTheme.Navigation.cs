using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
//<<<<<<< HEAD
        public Font NavigationTitleFont { get; set; } = new Font("Segoe UI", 16f, FontStyle.Bold);
        public Font NavigationSelectedFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Regular);
        public Font NavigationUnSelectedFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.White;
        public Color NavigationForeColor { get; set; } = Color.Black;
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color NavigationHoverForeColor { get; set; } = Color.DodgerBlue;
        public Color NavigationSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
