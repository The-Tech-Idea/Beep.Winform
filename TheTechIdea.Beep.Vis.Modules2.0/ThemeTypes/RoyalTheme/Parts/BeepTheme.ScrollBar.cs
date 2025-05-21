using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
    }
}