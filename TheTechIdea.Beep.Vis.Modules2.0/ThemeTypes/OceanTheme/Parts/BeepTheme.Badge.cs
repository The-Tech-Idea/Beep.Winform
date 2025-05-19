using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Badge Colors & Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color BadgeBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for badge background
        public Color BadgeForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for badge text
        public Color HighlightBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for highlight
        public TypographyStyle BadgeFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 10f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}