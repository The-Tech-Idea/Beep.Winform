using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Badge Colors & Fonts
//<<<<<<< HEAD
        public Color BadgeBackColor { get; set; } = Color.FromArgb(55, 71, 79); // dark slate gray tone
        public Color BadgeForeColor { get; set; } = Color.WhiteSmoke;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 87, 34); // a bright orange accent color
        public TypographyStyle  BadgeFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
    }
}
