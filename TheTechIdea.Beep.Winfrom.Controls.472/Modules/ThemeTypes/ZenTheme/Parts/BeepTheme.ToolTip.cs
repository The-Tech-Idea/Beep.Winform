using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color ToolTipForeColor { get; set; } = Color.White;
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(50, 0, 0, 0);
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(76, 0, 0, 0); // 30% opacity
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(156, 39, 176);
    }
}