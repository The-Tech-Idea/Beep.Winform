using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color StatusBarForeColor { get; set; } = Color.Black;
        public Color StatusBarBorderColor { get; set; } = Color.LightGray;

        public Color StatusBarHoverBackColor { get; set; } = Color.LightBlue;
        public Color StatusBarHoverForeColor { get; set; } = Color.Black;
        public Color StatusBarHoverBorderColor { get; set; } = Color.SteelBlue;
    }
}
