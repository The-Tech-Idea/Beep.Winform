using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(20, 20, 30);
        public Color StatusBarForeColor { get; set; } = Color.WhiteSmoke;
        public Color StatusBarBorderColor { get; set; } = Color.DarkSlateGray;
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(40, 40, 60);
        public Color StatusBarHoverForeColor { get; set; } = Color.White;
        public Color StatusBarHoverBorderColor { get; set; } = Color.LightSteelBlue;
    }
}
