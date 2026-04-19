using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public string FontName { get; set; } = "Roboto"; // Default font for theme
        public float FontSize { get; set; } = 8f; // Default font size in points
        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 8f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.5f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 8f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(150, 150, 160), // Light gray
            LineHeight = 1.1f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 8f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 8f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(80, 150, 200), // Soft blue
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = true,
            IsStrikeout = false
        };
        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 8f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(150, 150, 160), // Light gray
            LineHeight = 1.1f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}
