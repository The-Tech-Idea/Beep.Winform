using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Dark hover background
        public Color ToolTipForeColor { get; set; } = Color.White;
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Accent border
        public Color ToolTipShadowColor { get; set; } = Color.Black;
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(90, 0, 0, 0); // Semi-transparent black
        public Color ToolTipTextColor { get; set; } = Color.White;
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Link blue
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(0x6E, 0xD8, 0xFF); // Brighter on hover
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(0xA0, 0xA0, 0xFF); // Soft violet
    }
}
