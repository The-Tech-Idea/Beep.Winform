using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(245, 245, 245); // light gray background
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(160, 160, 160); // medium gray thumb
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(230, 230, 230); // lighter track
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(120, 120, 120); // darker thumb on hover
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(220, 220, 220); // slightly darker track on hover
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(100, 100, 100); // active thumb color (darker)
    }
}
