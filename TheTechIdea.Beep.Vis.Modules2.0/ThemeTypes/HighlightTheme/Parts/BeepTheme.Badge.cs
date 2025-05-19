using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Badge Colors & Fonts
<<<<<<< HEAD
        public Color BadgeBackColor { get; set; } = Color.FromArgb(255, 204, 0); // bright yellow
        public Color BadgeForeColor { get; set; } = Color.Black;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 255, 153); // pale yellow
        public Font BadgeFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
=======
        public Color BadgeBackColor { get; set; }
        public Color BadgeForeColor { get; set; }
        public Color HighlightBackColor { get; set; }
        public TypographyStyle BadgeFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
