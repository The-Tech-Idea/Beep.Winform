using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ToolTipForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(50, 0, 0, 0);
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(76, 0, 0, 0); // 30% opacity
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(135, 206, 250);
    }
}