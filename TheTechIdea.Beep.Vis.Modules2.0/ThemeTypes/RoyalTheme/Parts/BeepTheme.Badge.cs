using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color BadgeForeColor { get; set; } = Color.White;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public TypographyStyle BadgeFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}