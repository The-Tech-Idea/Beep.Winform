using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.Black;
        public Color BadgeForeColor { get; set; } = Color.White;
        public Color HighlightBackColor { get; set; } = Color.Yellow;
        public TypographyStyle  BadgeFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
    }
}
