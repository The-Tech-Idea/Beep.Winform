using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color StatusBarHoverForeColor { get; set; } = Color.White;
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
    }
}