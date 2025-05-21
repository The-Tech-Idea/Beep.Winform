using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color ToolTipForeColor { get; set; } = Color.White;
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(100, 0, 80, 120);
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(76, 0, 80, 120); // 30% opacity
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(0, 120, 170);
    }
}