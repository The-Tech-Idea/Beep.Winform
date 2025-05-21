using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for tooltip background
        public Color ToolTipForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for tooltip text
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for tooltip border
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(120, 26, 188, 156); // Semi-transparent neon turquoise for shadow
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(76, 0, 0, 0); // 30% opacity (76/255) for shadow
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for tooltip text
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple for links
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for link hover
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(142, 68, 173); // Slightly darker neon purple for visited links
    }
}