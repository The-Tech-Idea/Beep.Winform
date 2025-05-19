using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(50, 120, 160); // Deep sky blue for links
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(40, 100, 140); // Slightly darker blue for visited links
        public Color HoverLinkColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover
        public Color LinkHoverColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover (redundant with HoverLinkColor)
    }
}