using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color HoverLinkColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color LinkHoverColor { get; set; } = Color.FromArgb(255, 204, 221);
    }
}