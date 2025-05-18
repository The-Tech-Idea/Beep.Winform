using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(255, 250, 240);        // Light sand
        public Color ToolTipForeColor { get; set; } = Color.FromArgb(102, 51, 0);          // Dark brown
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(210, 180, 140);     // Tan border
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(128, 160, 82, 45);  // Semi-transparent sienna
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(64, 160, 82, 45); // More transparent sienna
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(102, 51, 0);          // Dark brown
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(244, 164, 96);        // Sandy brown
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(210, 105, 30);   // Chocolate
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(160, 82, 45);  // Sienna
    }
}
