using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(245, 235, 237);
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(245, 183, 203);
    }
}