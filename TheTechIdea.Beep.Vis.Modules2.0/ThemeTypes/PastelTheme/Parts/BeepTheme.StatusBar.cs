using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color StatusBarHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(247, 221, 229);
    }
}