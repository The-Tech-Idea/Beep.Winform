using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Badge Colors & Fonts
        // Note: Ensure 'Courier New' font family is available for retro aesthetic. If unavailable, 'Consolas' is a fallback.
        public Color BadgeBackColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal for badge background
        public Color BadgeForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for badge text
        public Color HighlightBackColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for highlight
        public TypographyStyle BadgeFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 10f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}