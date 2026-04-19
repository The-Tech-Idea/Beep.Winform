using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(40, 30, 60); // Dark purple-gray for neon contrast
        public Color BadgeForeColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for text
        public Color HighlightBackColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple for highlights
        public TypographyStyle BadgeFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold); // Consistent with theme
    }
}
