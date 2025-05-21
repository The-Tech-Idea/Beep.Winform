using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color ToolTipForeColor { get; set; } = Color.White;
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(128, 0, 0, 0);
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(128, 0, 0, 0); // Assuming opacity as a color with alpha
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(255, 192, 64);
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(192, 128, 0);
    }
}