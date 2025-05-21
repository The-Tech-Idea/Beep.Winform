using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(30, 30, 30);        // Very dark background
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(70, 70, 70);       // Dark gray thumb
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(45, 45, 45);       // Slightly lighter track
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(100, 100, 100); // Light gray on hover
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(60, 60, 60);  // Lighter track on hover
        public Color ScrollBarActiveThumbColor { get; set; } = Color.DodgerBlue;           // Bright blue active thumb
    }
}
