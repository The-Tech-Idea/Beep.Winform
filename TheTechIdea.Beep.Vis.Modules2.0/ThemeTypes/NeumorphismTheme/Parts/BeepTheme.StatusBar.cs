using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for status bar background
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for status bar text
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for status bar border
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for hover background
        public Color StatusBarHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover text
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover border
    }
}