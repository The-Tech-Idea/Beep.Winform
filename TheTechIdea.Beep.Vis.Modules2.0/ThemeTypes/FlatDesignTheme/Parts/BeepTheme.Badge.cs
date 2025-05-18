using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        public Color BadgeBackColor { get; set; } = Color.FromArgb(255, 87, 34); // Deep orange
        public Color BadgeForeColor { get; set; } = Color.White;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 193, 7); // Amber
        public Font BadgeFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
    }
}
