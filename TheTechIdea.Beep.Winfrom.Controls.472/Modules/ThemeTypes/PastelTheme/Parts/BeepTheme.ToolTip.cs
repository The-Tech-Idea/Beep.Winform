using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color ToolTipForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(100, 237, 181, 201);
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(76, 237, 181, 201); // 30% opacity
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(237, 181, 201);
    }
}