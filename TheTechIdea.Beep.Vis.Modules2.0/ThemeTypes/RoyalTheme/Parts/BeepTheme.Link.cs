using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(153, 102, 255);
        public Color HoverLinkColor { get; set; } = Color.FromArgb(255, 213, 27);
        public Color LinkHoverColor { get; set; } = Color.FromArgb(255, 213, 27);
    }
}