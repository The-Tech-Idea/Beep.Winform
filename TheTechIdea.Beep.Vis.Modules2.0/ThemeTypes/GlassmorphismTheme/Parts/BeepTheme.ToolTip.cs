using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.LightYellow;
        public Color ToolTipForeColor { get; set; } = Color.Black;
        public Color ToolTipBorderColor { get; set; } = Color.Gray;
        public Color ToolTipShadowColor { get; set; } = Color.DarkGray;
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(80, 0, 0, 0); // semi-transparent black, but not fully transparent
        public Color ToolTipTextColor { get; set; } = Color.Black;
        public Color ToolTipLinkColor { get; set; } = Color.Blue;
        public Color ToolTipLinkHoverColor { get; set; } = Color.MediumBlue;
        public Color ToolTipLinkVisitedColor { get; set; } = Color.Purple;
    }
}
