using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
        public Color BadgeForeColor { get; set; } = Color.White;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(60, 179, 113); // MediumSeaGreen
        public Font BadgeFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
    }
}
