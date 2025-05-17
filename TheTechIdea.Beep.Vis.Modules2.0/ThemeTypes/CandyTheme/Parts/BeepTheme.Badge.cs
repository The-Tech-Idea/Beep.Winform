using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(255, 182, 193);    // Light Candy Pink
        public Color BadgeForeColor { get; set; } = Color.White;                      // White for contrast
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 223, 93); // Candy Lemon (highlighted badge)

        // You can substitute with "Comic Sans MS", "Segoe UI Semibold", or "Nunito" if available for a more rounded, playful effect
        public Font BadgeFont { get; set; } = new Font("Comic Sans MS", 10.5f, FontStyle.Bold);
    }
}
