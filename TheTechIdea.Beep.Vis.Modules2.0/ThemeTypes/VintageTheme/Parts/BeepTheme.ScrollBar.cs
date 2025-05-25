using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(240, 235, 215);
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(230, 225, 205);
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(160, 82, 45);
    }
}