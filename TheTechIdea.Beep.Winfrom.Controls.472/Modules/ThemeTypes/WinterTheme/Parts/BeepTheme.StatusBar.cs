using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color StatusBarForeColor { get; set; } = Color.White;
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(60, 100, 140);
        public Color StatusBarHoverForeColor { get; set; } = Color.White;
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
    }
}