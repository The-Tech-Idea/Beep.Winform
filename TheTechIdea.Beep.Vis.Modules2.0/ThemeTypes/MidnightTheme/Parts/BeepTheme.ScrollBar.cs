using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(100, 100, 100);
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(130, 130, 130);
    }
}
