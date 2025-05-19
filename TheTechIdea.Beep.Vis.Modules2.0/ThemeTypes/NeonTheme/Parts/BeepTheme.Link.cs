using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple for links
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(142, 68, 173); // Slightly darker neon purple for visited links
        public Color HoverLinkColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover
        public Color LinkHoverColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover (redundant with HoverLinkColor)
    }
}