using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(96, 195, 100);
    }
}