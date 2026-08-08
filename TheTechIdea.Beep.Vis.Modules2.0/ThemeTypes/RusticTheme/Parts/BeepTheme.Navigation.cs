using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle NavigationTitleFont { get; set; }
        public TypographyStyle NavigationSelectedFont { get; set; }
        public TypographyStyle NavigationUnSelectedFont { get; set; }

        public Color NavigationBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color NavigationForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}
