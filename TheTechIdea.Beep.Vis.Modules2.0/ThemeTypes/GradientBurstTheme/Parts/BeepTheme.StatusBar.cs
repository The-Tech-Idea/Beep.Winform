using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(40, 44, 52);
        public Color StatusBarForeColor { get; set; } = Color.White;
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(60, 63, 65);
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(55, 60, 70);
        public Color StatusBarHoverForeColor { get; set; } = Color.LightCyan;
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(80, 85, 90);
    }
}
