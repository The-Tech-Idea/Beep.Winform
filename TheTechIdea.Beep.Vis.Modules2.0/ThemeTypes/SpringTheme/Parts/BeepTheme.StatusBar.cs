using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color StatusBarHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
    }
}