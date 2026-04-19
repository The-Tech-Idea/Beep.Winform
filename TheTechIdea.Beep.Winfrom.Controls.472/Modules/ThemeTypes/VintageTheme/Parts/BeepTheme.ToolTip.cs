using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(240, 235, 215);
        public Color ToolTipForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(50, 25, 0);
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(76, 0, 0, 0); // 30% opacity
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(139, 69, 19);
    }
}