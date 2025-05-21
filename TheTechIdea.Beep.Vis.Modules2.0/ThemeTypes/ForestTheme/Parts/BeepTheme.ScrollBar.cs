using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(34, 49, 34); // Dark forest green
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(56, 142, 60); // Medium green thumb
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(46, 71, 46); // Darker green track
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(76, 175, 80); // Hover bright green thumb
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(56, 142, 60); // Hover track lighter green
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(102, 187, 106); // Active bright green thumb
    }
}
