using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(240, 240, 240); // Light gray background
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(50, 50, 50); // Dark gray text
        public Color StatusBarBorderColor { get; set; } = Color.LightGray; // Subtle border
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230); // Slightly darker on hover
        public Color StatusBarHoverForeColor { get; set; } = Color.Black; // Black text on hover
        public Color StatusBarHoverBorderColor { get; set; } = Color.Gray; // Darker border on hover
    }
}
