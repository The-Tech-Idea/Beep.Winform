using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Badge Colors & Fonts - warm desert tones
        public Color BadgeBackColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color BadgeForeColor { get; set; } = Color.FromArgb(60, 30, 10); // Dark Brown
        public Color HighlightBackColor { get; set; } = Color.FromArgb(244, 164, 96); // Sandy Brown
        public Font BadgeFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
    }
}
