using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(230, 240, 245);
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(60, 179, 113);
    }
}