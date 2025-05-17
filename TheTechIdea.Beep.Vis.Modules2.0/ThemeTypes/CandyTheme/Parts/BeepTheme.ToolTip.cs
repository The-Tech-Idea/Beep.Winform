using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // ToolTip Colors

        public Color ToolTipBackColor { get; set; } = Color.FromArgb(255, 253, 194);      // Lemon Yellow
        public Color ToolTipForeColor { get; set; } = Color.FromArgb(44, 62, 80);         // Navy
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(127, 255, 212);    // Mint

        // Subtle candy pink shadow, slightly transparent
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(80, 240, 100, 180); // Semi-transparent Candy Pink
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(80, 240, 100, 180); // (for compatibility, use same as shadow color)

        public Color ToolTipTextColor { get; set; } = Color.FromArgb(44, 62, 80);          // Navy (for clarity)
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(54, 162, 235);        // Soft Blue
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(240, 100, 180);  // Candy Pink
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(206, 183, 255);// Pastel Lavender
    }
}
