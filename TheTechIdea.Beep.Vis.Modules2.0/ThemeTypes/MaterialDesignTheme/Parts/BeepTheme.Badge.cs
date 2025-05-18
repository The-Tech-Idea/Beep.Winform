using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(244, 67, 54); // Material Red 500
        public Color BadgeForeColor { get; set; } = Color.White;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 235, 59); // Material Yellow 500
        public Font BadgeFont { get; set; } = new Font("Roboto", 10f, FontStyle.Bold);
    }
}
