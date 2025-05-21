using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(255, 255, 240);
        public Color ToolTipForeColor { get; set; } = Color.Black;
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ToolTipShadowColor { get; set; } = Color.Gray;
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(100, 0, 0, 0); // Approximate 40% opacity black
        public Color ToolTipTextColor { get; set; } = Color.Black;
        public Color ToolTipLinkColor { get; set; } = Color.Blue;
        public Color ToolTipLinkHoverColor { get; set; } = Color.DarkBlue;
        public Color ToolTipLinkVisitedColor { get; set; } = Color.Purple;
    }
}
