using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(45, 45, 128); // Darker royal blue
        public Color StatusBarHoverForeColor { get; set; } = Color.White;
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
    }
}