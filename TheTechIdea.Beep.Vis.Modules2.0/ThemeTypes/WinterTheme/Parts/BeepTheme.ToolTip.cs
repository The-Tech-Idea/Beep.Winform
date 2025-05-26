using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(45, 85, 120);
        public Color ToolTipForeColor { get; set; } = Color.White;
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(50, 0, 0, 0);
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(76, 0, 0, 0); // 30% opacity
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(120, 169, 255);
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(156, 39, 176);
    }
}