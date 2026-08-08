using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // ToolTip Colors — dark walnut card, beige ink, goldenrod links (the theme's
        // rustic identity). These slots carried NO initializers, so every tooltip under
        // Rustic resolved Color.Empty and rendered transparent-on-transparent.
        public Color ToolTipBackColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color ToolTipForeColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color ToolTipBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ToolTipShadowColor { get; set; } = Color.FromArgb(96, 0, 0, 0);
        public Color ToolTipShadowOpacity { get; set; } = Color.FromArgb(96, 0, 0, 0);
        public Color ToolTipTextColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color ToolTipLinkColor { get; set; } = Color.FromArgb(184, 134, 11);
        public Color ToolTipLinkHoverColor { get; set; } = Color.FromArgb(218, 165, 32);
        public Color ToolTipLinkVisitedColor { get; set; } = Color.FromArgb(205, 133, 63);
    }
}
