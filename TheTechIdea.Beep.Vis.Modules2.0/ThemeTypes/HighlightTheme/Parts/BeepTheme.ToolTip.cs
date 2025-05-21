using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.LightYellow;
        public Color ToolTipForeColor { get; set; } = Color.Black;
        public Color ToolTipBorderColor { get; set; } = Color.Goldenrod;
        public Color ToolTipShadowColor { get; set; } = Color.Gray;
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(128, Color.Black);
        public Color ToolTipTextColor { get; set; } = Color.Black;
        public Color ToolTipLinkColor { get; set; } = Color.Blue;
        public Color ToolTipLinkHoverColor { get; set; } = Color.DarkBlue;
        public Color ToolTipLinkVisitedColor { get; set; } = Color.Purple;
    }
}
