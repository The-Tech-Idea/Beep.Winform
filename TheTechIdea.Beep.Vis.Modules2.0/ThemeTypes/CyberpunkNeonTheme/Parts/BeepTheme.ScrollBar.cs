using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // ScrollBar Colors

        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(18, 18, 32);        // Cyberpunk Black (scrollbar background)
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(0, 255, 255);      // Neon Cyan (thumb)
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(34, 34, 68);       // Slightly lighter cyber-black (track)

        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(255, 255, 0); // Neon Yellow (hover thumb)
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(0, 255, 128); // Neon Green (hover track)
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(255, 0, 255);// Neon Magenta (active thumb)
    }
}
