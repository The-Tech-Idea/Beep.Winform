using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(180, 200, 230); // Soft blue
        public Color BadgeForeColor { get; set; } = Color.Black;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 220, 120); // Warm highlight
        public TypographyStyle  BadgeFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Bold);
    }
}
