using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color ToolTipForeColor { get; set; } = Color.LightGray;
        public Color ToolTipBorderColor { get; set; } = Color.DimGray;
        public Color ToolTipShadowColor { get; set; } = Color.Black;
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(100, 0, 0, 0); // Semi-transparent black
        public Color ToolTipTextColor { get; set; } = Color.Gainsboro;
        public Color ToolTipLinkColor { get; set; } = Color.CornflowerBlue;
        public Color ToolTipLinkHoverColor { get; set; } = Color.DeepSkyBlue;
        public Color ToolTipLinkVisitedColor { get; set; } = Color.MediumSlateBlue;
    }
}
