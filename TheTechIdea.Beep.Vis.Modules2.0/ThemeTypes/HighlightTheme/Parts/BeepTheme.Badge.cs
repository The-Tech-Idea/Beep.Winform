using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Badge Colors & Fonts
//<<<<<<< HEAD
        public Color BadgeBackColor { get; set; } = Color.FromArgb(255, 204, 0); // bright yellow
        public Color BadgeForeColor { get; set; } = Color.Black;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 255, 153); // pale yellow
        public Font BadgeFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
    }
}
