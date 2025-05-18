using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(0x44, 0x44, 0x66); // Dark violet-gray
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(0x2A, 0x2A, 0x40); // Dim violet
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(0x66, 0x66, 0x99); // Brighter thumb on hover
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x55); // Subtle hover effect
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Sky blue (active state)
    }
}
