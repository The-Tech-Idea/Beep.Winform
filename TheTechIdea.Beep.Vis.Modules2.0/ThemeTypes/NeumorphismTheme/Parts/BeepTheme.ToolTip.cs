using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for tooltip background
        public Color ToolTipForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for tooltip text
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for tooltip border
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(120, 200, 200, 205); // Semi-transparent soft gray for shadow
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(76, 0, 0, 0); // 30% opacity (76/255) for shadow
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for tooltip text
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(80, 150, 200); // Soft blue for links
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for link hover
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(70, 130, 180); // Slightly darker blue for visited links
    }
}