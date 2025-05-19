using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color StatusBarForeColor { get; set; } = Color.White;
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color StatusBarHoverForeColor { get; set; } = Color.White;
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
    }
}