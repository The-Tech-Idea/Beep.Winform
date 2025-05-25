using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color StatusBarHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
    }
}