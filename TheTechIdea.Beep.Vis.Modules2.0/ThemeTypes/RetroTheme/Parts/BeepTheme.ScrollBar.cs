using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(181, 137, 0);
    }
}