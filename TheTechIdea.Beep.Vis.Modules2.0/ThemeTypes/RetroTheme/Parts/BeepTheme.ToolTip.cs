using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color ToolTipForeColor { get; set; } = Color.White;
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(128, 88, 110, 117);
        public Color ToolTipTextColor { get; set; } = Color.White;
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(108, 113, 196);
    }
}