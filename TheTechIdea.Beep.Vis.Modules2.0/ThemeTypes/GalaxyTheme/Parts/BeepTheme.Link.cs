using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Light blue
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(0xA0, 0xA0, 0xFF); // Soft violet
        public Color HoverLinkColor { get; set; } = Color.FromArgb(0x6E, 0xD8, 0xFF); // Brighter blue
        public Color LinkHoverColor { get; set; } = Color.FromArgb(0x23, 0xB9, 0xF5); // Cyan tint
    }
}
