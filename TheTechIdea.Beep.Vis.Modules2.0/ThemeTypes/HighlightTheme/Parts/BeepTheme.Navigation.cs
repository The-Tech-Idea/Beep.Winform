using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public Font NavigationTitleFont { get; set; }
        public Font NavigationSelectedFont { get; set; }
        public Font NavigationUnSelectedFont { get; set; }

        public Color NavigationBackColor { get; set; }
        public Color NavigationForeColor { get; set; }
        public Color NavigationHoverBackColor { get; set; }
        public Color NavigationHoverForeColor { get; set; }
        public Color NavigationSelectedBackColor { get; set; }
        public Color NavigationSelectedForeColor { get; set; }
    }
}
