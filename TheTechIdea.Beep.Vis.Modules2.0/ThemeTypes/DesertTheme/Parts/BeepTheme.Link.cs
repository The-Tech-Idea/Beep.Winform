using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(198, 134, 66);          // Desert orange
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(153, 102, 51);   // Rich brown
        public Color HoverLinkColor { get; set; } = Color.FromArgb(244, 164, 96);     // Warm light orange
        public Color LinkHoverColor { get; set; } = Color.FromArgb(210, 180, 140);    // Tan
    }
}
