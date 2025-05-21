using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(40, 40, 40);  // Dark gray background
        public Color ScrollBarThumbColor { get; set; } = Color.Gray;                  // Normal thumb
        public Color ScrollBarTrackColor { get; set; } = Color.DimGray;               // Track behind thumb
        public Color ScrollBarHoverThumbColor { get; set; } = Color.LightGray;        // Hover effect on thumb
        public Color ScrollBarHoverTrackColor { get; set; } = Color.Gray;             // Hover effect on track
        public Color ScrollBarActiveThumbColor { get; set; } = Color.White;           // Active thumb color
    }
}
