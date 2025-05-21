using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.SteelBlue;
        public Color VisitedLinkColor { get; set; } = Color.Purple;
        public Color HoverLinkColor { get; set; } = Color.DeepSkyBlue;
        public Color LinkHoverColor { get; set; } = Color.DeepSkyBlue; // Same as HoverLinkColor for consistency
    }
}
