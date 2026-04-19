using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color BadgeForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color HighlightBackColor { get; set; } = Color.FromArgb(50, 205, 50);
        public TypographyStyle BadgeFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.1f,
            FontWeight = FontWeight.SemiBold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}