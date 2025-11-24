using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(255, 204, 0); // bright yellow
        public Color BadgeForeColor { get; set; } = Color.Black;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 255, 153); // pale yellow
        public TypographyStyle  BadgeFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
    }
}
