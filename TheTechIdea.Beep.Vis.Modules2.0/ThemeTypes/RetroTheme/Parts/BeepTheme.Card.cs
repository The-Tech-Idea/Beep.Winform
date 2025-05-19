using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Card Colors & Fonts
        // Note: Ensure 'Courier New' font family is available for retro aesthetic. If unavailable, 'Consolas' is a fallback.
        public TypographyStyle CardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(255, 215, 0), // Retro yellow
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CardTextForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for text
        public Color CardBackColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for card background
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for title
        public TypographyStyle CardSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 14f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(192, 192, 192), // Light gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(192, 192, 192); // Light gray for subtitle
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(0, 255, 255), // Bright cyan
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.5f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 14f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(192, 192, 192), // Light gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(0, 43, 43); // Darker teal
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(0, 64, 64); // Mid-tone teal
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for retro CRT effect
    }
}