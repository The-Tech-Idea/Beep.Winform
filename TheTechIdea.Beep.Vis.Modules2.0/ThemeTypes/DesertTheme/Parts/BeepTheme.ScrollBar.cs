using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(245, 235, 220); // Light sand beige
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan for thumb
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(255, 248, 220); // Cornsilk track

        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(244, 164, 96); // SandyBrown hover thumb
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(255, 239, 213); // PapayaWhip hover track

        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru active thumb
    }
}
