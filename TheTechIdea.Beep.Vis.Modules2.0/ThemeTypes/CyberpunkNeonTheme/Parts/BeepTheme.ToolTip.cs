using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // ToolTip Colors
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(20, 20, 40);             // Dark translucent background
        public Color ToolTipForeColor { get; set; } = Color.White;                           // White text
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(0, 255, 255);         // Neon cyan border
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(0, 255, 255);         // Neon cyan shadow
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(80, 0, 255, 255);   // Semi-transparent neon cyan
        public Color ToolTipTextColor { get; set; } = Color.White;                           // Text color

        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(255, 0, 255);           // Neon magenta links
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(255, 255, 0);      // Neon yellow hover
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(0, 255, 128);    // Neon green visited links
    }
}
