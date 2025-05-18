using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color VisitedLinkColor { get; set; } = Color.Purple;
        public Color HoverLinkColor { get; set; } = Color.FromArgb(10, 130, 230);
        public Color LinkHoverColor { get; set; } = Color.FromArgb(10, 130, 230);
    }
}
