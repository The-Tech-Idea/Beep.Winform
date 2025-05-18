using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public Font NavigationTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font NavigationSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font NavigationUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.White;
        public Color NavigationForeColor { get; set; } = Color.FromArgb(33, 37, 41); // dark charcoal
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230); // light gray hover
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(0, 120, 215); // accent blue hover
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215); // accent blue selected
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
