using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(0, 120, 215);           // Bright blue
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(104, 33, 122);    // Purple tone for visited links
        public Color HoverLinkColor { get; set; } = Color.FromArgb(0, 84, 153);       // Darker blue on hover
        public Color LinkHoverColor { get; set; } = Color.FromArgb(0, 84, 153);        // Same as hover for consistency
    }
}
