using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for scrollbar background
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for thumb
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for track
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover thumb
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover track
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(80, 180, 160); // Slightly darker teal for active thumb
    }
}