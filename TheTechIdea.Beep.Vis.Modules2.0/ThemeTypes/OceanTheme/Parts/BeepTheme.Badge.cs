using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color BadgeForeColor { get; set; } = Color.White;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public TypographyStyle BadgeFont { get; set; } = new TypographyStyle() { FontSize = 10, FontWeight = FontWeight.Medium, TextColor = Color.White };
    }
}