using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(255, 255, 225); // Light yellowish
        public Color ToolTipForeColor { get; set; } = Color.Black;
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ToolTipShadowColor { get; set; } = Color.Gray;
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(100, 0, 0, 0); // semi-transparent black
        public Color ToolTipTextColor { get; set; } = Color.Black;
        public Color ToolTipLinkColor { get; set; } = Color.DodgerBlue;
        public Color ToolTipLinkHoverColor { get; set; } = Color.MediumBlue;
        public Color ToolTipLinkVisitedColor { get; set; } = Color.Purple;
    }
}
