using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color BadgeForeColor { get; set; } = Color.WhiteSmoke;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 105, 180); // Hot Pink highlight
        public Font BadgeFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Bold);
    }
}
