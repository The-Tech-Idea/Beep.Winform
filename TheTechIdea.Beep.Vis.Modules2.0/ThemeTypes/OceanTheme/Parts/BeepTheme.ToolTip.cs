using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for tooltip background
        public Color ToolTipForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for tooltip text
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for tooltip border
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(120, 100, 200, 180); // Semi-transparent bright teal for shadow
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(76, 0, 0, 0); // 30% opacity (76/255) for shadow
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for tooltip text
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(50, 120, 160); // Deep sky blue for links
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for link hover
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(40, 100, 140); // Slightly darker sky blue for visited links
    }
}