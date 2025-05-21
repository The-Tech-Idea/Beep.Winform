using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(240, 255, 240); // light green
        public Color ToolTipForeColor { get; set; } = Color.DarkGreen;
        public Color ToolTipBorderColor { get; set; } = Color.ForestGreen;
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(100, 0, 50, 0); // semi-transparent dark green shadow
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(50, 0, 0, 0); // subtle black shadow overlay
        public Color ToolTipTextColor { get; set; } = Color.DarkGreen;
        public Color ToolTipLinkColor { get; set; } = Color.SeaGreen;
        public Color ToolTipLinkHoverColor { get; set; } = Color.OliveDrab;
        public Color ToolTipLinkVisitedColor { get; set; } = Color.MediumSeaGreen;
    }
}
