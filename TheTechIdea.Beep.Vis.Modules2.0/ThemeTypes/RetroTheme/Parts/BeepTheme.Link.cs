using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(108, 113, 196);
        public Color HoverLinkColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color LinkHoverColor { get; set; } = Color.FromArgb(181, 137, 0);
    }
}