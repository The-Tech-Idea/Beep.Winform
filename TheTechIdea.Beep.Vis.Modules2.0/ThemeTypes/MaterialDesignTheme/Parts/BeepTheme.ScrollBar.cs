using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // ScrollBar Colors with Material Design defaults
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(245, 245, 245);         // Light gray background
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(158, 158, 158);         // Medium gray thumb
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(224, 224, 224);         // Slightly lighter track
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(117, 117, 117);    // Darker gray on hover
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(238, 238, 238);    // Lighter track on hover
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(33, 150, 243);    // Blue active thumb
    }
}
