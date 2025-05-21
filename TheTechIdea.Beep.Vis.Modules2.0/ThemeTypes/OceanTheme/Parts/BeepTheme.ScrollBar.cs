using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(240, 245, 250);
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(230, 240, 245);
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(0, 160, 210);
    }
}