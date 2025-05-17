using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Link (TextBox Link) colors

        public Color LinkColor { get; set; } = Color.FromArgb(0, 255, 255);           // Neon Cyan
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(255, 0, 255);    // Neon Magenta
        public Color HoverLinkColor { get; set; } = Color.FromArgb(255, 255, 0);      // Neon Yellow
        public Color LinkHoverColor { get; set; } = Color.FromArgb(0, 255, 128);      // Neon Green
    }
}
