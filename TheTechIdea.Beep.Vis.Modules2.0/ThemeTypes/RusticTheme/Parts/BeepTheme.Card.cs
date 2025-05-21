using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Card Colors & Fonts
        public TypographyStyle CardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.4f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CardTextForeColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public Color CardBackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public TypographyStyle CardSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(139, 69, 19), // SaddleBrown
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(139, 69, 19), // SaddleBrown
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(227, 212, 180); // Midpoint Beige
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}