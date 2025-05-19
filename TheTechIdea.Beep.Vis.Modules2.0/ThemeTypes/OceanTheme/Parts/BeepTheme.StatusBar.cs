using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for status bar background
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for status bar text
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for status bar border
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover background
        public Color StatusBarHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover text
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover border
    }
}