using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color BadgeForeColor { get; set; } = Color.White;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 204, 221);
        public TypographyStyle BadgeFont { get; set; } = new TypographyStyle() { FontSize = 10, FontWeight = FontWeight.Medium, TextColor = Color.White };
    }
}