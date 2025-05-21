using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
//<<<<<<< HEAD
        // Navigation & Breadcrumbs Fonts & Colors with defaults
        public Font NavigationTitleFont { get; set; } = new Font("Roboto", 16f, FontStyle.Bold);
        public Font NavigationSelectedFont { get; set; } = new Font("Roboto", 14f, FontStyle.Bold);
        public Font NavigationUnSelectedFont { get; set; } = new Font("Roboto", 14f, FontStyle.Regular);

        public Color NavigationBackColor { get; set; } = Color.FromArgb(250, 250, 250); // Light gray background
        public Color NavigationForeColor { get; set; } = Color.FromArgb(66, 66, 66); // Dark gray text
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(224, 224, 224); // Slightly darker hover background
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue accent hover text
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue selected background
        public Color NavigationSelectedForeColor { get; set; } = Color.White; // White text on selected
    }
}
