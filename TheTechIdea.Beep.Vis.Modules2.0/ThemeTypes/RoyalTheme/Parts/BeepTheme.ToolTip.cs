using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color ToolTipForeColor { get; set; } = Color.White;
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(100, 0, 0, 0);
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(76, 0, 0, 0); // 30% opacity
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(255, 213, 27);
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(153, 102, 255);
    }
}