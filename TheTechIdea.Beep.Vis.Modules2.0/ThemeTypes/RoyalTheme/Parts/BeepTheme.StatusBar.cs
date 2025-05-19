using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color StatusBarForeColor { get; set; } = Color.White;
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color StatusBarHoverForeColor { get; set; } = Color.White;
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
    }
}