using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(32, 32, 32);
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(160, 160, 160);
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(255, 165, 0);
    }
}