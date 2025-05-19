using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for tooltip background
        public Color ToolTipForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for tooltip text
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for tooltip border
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(120, 180, 200, 220); // Semi-transparent pastel lavender for shadow
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(76, 0, 0, 0); // 30% opacity (76/255) for shadow
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for tooltip text
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for links
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for link hover
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(100, 140, 170); // Slightly darker pastel blue for visited links
    }
}