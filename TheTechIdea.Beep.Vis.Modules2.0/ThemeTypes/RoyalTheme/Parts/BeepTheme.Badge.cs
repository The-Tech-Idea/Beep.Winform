using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color BadgeForeColor { get; set; } = Color.White;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public TypographyStyle BadgeFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}