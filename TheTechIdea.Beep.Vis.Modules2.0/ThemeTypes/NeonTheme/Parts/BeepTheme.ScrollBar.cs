using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for scrollbar background
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for thumb
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for track
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover thumb
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover track
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for active thumb
    }
}