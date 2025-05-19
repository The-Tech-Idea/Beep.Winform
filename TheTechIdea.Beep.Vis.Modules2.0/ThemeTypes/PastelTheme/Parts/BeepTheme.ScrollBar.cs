using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for scrollbar background
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for thumb
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for track
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover thumb
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover track
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for active thumb
    }
}