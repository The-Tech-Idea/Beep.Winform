using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(240, 240, 240);  // Light gray background
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(160, 160, 160); // Medium gray thumb
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(220, 220, 220); // Slightly lighter track
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(120, 120, 120); // Darker thumb on hover
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(210, 210, 210); // Slightly darker track on hover
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(90, 90, 90);  // Darkest thumb when active
    }
}
