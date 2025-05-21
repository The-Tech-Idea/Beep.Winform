using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(45, 45, 128); // Darker royal blue
        public Color HoverLinkColor { get; set; } = Color.FromArgb(100, 100, 180); // Light indigo
        public Color LinkHoverColor { get; set; } = Color.FromArgb(100, 100, 180); // Light indigo
    }
}