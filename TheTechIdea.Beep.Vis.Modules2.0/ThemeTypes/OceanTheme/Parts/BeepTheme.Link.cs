using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color HoverLinkColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color LinkHoverColor { get; set; } = Color.FromArgb(0, 180, 230);
    }
}