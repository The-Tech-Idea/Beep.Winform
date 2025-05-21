using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(192, 128, 0);
        public Color HoverLinkColor { get; set; } = Color.FromArgb(255, 192, 64);
        public Color LinkHoverColor { get; set; } = Color.FromArgb(255, 192, 64);
    }
}