using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.Black;
        public Color ToolTipForeColor { get; set; } = Color.White;
        public Color ToolTipBorderColor { get; set; } = Color.White;
        public Color ToolTipShadowColor { get; set; } = Color.Gray;
        public Color ToolTipShadowOpacity { get; set; } = Color.DimGray;
        public Color ToolTipTextColor { get; set; } = Color.White;
        public Color ToolTipLinkColor { get; set; } = Color.Cyan;
        public Color ToolTipLinkHoverColor { get; set; } = Color.Yellow;
        public Color ToolTipLinkVisitedColor { get; set; } = Color.Magenta;
    }
}
