using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color StatusBarForeColor { get; set; } = Color.White;
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color StatusBarHoverForeColor { get; set; } = Color.White;
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
    }
}