using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Badge Colors & Fonts
        public Color BadgeBackColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color BadgeForeColor { get; set; } = Color.White;
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 165, 0);
        public TypographyStyle BadgeFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}