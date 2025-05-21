using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {

        public Color BadgeBackColor { get; set; } = Color.FromArgb(70, 130, 180); // SteelBlue
        public Color BadgeForeColor { get; set; } = Color.White;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(135, 206, 235); // SkyBlue
        public Font BadgeFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);

    }
}
