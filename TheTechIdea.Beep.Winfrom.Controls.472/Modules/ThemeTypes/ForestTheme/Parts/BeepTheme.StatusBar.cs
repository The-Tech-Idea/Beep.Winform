using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
        public Color StatusBarForeColor { get; set; } = Color.White;
        public Color StatusBarBorderColor { get; set; } = Color.DarkGreen;
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(46, 139, 87); // MediumSeaGreen
        public Color StatusBarHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color StatusBarHoverBorderColor { get; set; } = Color.LimeGreen;
    }
}
