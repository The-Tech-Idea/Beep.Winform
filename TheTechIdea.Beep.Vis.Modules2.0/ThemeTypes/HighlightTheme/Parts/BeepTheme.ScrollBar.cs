using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(180, 180, 180);
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(210, 210, 210);
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(150, 150, 150);
    }
}
