using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Badge Colors & Fonts

        // Badges: Neon magenta (main), black background, cyan highlights, bold font
        public Color BadgeBackColor { get; set; } = Color.FromArgb(24, 24, 48);          // Cyberpunk Black/Dark
        public Color BadgeForeColor { get; set; } = Color.FromArgb(255, 0, 255);         // Neon Magenta
        public Color HighlightBackColor { get; set; } = Color.FromArgb(0, 255, 255);     // Neon Cyan
        public TypographyStyle BadgeFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold);
    }
}
