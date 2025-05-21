using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Badge Colors & Fonts with default values
        public Color BadgeBackColor { get; set; } = Color.DimGray;
        public Color BadgeForeColor { get; set; } = Color.WhiteSmoke;
        public Color HighlightBackColor { get; set; } = Color.LightGray;
        public TypographyStyle BadgeFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
    }
}
