using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.LightYellow;
        public Color BadgeForeColor { get; set; } = Color.DarkSlateGray;
        public Color HighlightBackColor { get; set; } = Color.LightGoldenrodYellow;
        public TypographyStyle  BadgeFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Bold);
    }
}
