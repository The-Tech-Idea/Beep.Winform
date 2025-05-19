using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Card Colors & Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle CardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CardTextForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for text
        public Color CardBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for card background
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for title
        public TypographyStyle CardSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(100, 100, 100); // Medium gray for subtitle
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.5f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(220, 215, 230); // Pastel lavender
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for soft pastel effect
    }
}