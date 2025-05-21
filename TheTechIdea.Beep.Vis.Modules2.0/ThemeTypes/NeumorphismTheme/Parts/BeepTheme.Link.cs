using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(80, 150, 200); // Soft blue for links
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(70, 130, 180); // Slightly darker blue for visited links
        public Color HoverLinkColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover
        public Color LinkHoverColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover (redundant with HoverLinkColor)
    }
}