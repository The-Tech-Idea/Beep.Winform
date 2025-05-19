using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Bright blue background
        public Color BadgeForeColor { get; set; } = Color.White;                 // White text for contrast
        public Color HighlightBackColor { get; set; } = Color.FromArgb(100, 181, 246); // Lighter blue highlight
        public TypographyStyle BadgeFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
    }
}
