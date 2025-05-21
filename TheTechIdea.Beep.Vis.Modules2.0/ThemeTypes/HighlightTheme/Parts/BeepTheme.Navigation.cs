using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
//<<<<<<< HEAD
        public Font NavigationTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font NavigationSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font NavigationUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.FromArgb(250, 250, 250);
        public Color NavigationForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
