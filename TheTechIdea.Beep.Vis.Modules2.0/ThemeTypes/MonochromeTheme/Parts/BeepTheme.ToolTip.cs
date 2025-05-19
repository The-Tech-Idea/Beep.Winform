using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.WhiteSmoke;
        public Color ToolTipForeColor { get; set; } = Color.Black;
        public Color ToolTipBorderColor { get; set; } = Color.Gray;
        public Color ToolTipShadowColor { get; set; } = Color.DarkGray;
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(128, 0, 0, 0); // 50% opacity black
        public Color ToolTipTextColor { get; set; } = Color.Black;
        public Color ToolTipLinkColor { get; set; } = Color.DimGray;
        public Color ToolTipLinkHoverColor { get; set; } = Color.Black;
        public Color ToolTipLinkVisitedColor { get; set; } = Color.Gray;
    }
}
