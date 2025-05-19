using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Badge Colors & Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color BadgeBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for badge background
        public Color BadgeForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for badge text
        public Color HighlightBackColor { get; set; } = Color.FromArgb(200, 200, 205); // Slightly darker gray for highlight
        public TypographyStyle BadgeFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 10f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}