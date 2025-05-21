using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for status bar background
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for status bar text
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for status bar border
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover background
        public Color StatusBarHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover border
    }
}