using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for scrollbar background
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for thumb
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(220, 220, 225); // Slightly darker gray for track
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover thumb
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(210, 210, 215); // Darker gray for hover track
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(80, 160, 80); // Darker green for active thumb
    }
}