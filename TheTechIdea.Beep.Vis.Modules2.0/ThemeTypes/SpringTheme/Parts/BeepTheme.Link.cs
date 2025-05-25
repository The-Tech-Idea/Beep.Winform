using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(135, 206, 250);
        public Color HoverLinkColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color LinkHoverColor { get; set; } = Color.FromArgb(50, 205, 50);
    }
}