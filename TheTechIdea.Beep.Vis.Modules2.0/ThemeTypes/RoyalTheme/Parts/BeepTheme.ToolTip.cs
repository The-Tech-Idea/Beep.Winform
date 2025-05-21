using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color ToolTipForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(128, 0, 0, 0); // Black with opacity
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(128, 0, 0, 0); // Black with opacity
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(100, 100, 180); // Light indigo
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(45, 45, 128); // Darker royal blue
    }
}