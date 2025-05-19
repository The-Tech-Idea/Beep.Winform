using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for links
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(100, 140, 170); // Slightly darker pastel blue for visited links
        public Color HoverLinkColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for hover
        public Color LinkHoverColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for hover (redundant with HoverLinkColor)
    }
}