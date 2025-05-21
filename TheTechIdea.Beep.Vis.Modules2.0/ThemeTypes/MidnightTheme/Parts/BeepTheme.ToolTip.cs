using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(30, 30, 40);
        public Color ToolTipForeColor { get; set; } = Color.LightGray;
        public Color ToolTipBorderColor { get; set; } = Color.DimGray;
        public Color ToolTipShadowColor { get; set; } = Color.Black;
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(120, 0, 0, 0); // Semi-transparent black shadow
        public Color ToolTipTextColor { get; set; } = Color.WhiteSmoke;
        public Color ToolTipLinkColor { get; set; } = Color.CornflowerBlue;
        public Color ToolTipLinkHoverColor { get; set; } = Color.DodgerBlue;
        public Color ToolTipLinkVisitedColor { get; set; } = Color.MediumSlateBlue;
    }
}
