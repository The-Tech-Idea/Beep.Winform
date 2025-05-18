using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(34, 139, 34);           // ForestGreen
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(85, 107, 47);    // DarkOliveGreen
        public Color HoverLinkColor { get; set; } = Color.FromArgb(60, 179, 113);     // MediumSeaGreen
        public Color LinkHoverColor { get; set; } = Color.FromArgb(46, 139, 87);      // SeaGreen
    }
}
