using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(101, 67, 33); // Dark Brown
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(244, 164, 96); // SandyBrown
        public Color StatusBarHoverForeColor { get; set; } = Color.FromArgb(69, 33, 12); // Darker Brown
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
    }
}
