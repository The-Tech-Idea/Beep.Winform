using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // ToolTip Colors with Material Design defaults
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(97, 97, 97); // Grey 700
        public Color ToolTipForeColor { get; set; } = Color.White;
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(66, 66, 66); // Darker Grey
        public Color ToolTipShadowColor { get; set; } = Color.Black;
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(100, 0, 0, 0); // Semi-transparent black
        public Color ToolTipTextColor { get; set; } = Color.White;
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(3, 169, 244); // Blue 500
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(2, 136, 209); // Blue 700
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(0, 188, 212); // Cyan 500
    }
}
