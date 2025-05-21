using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Switch control Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle SwitchTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SwitchSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SwitchUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SwitchBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for switch background
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for border
        public Color SwitchForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for switch text
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected background
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected border
        public Color SwitchSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for selected text
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for hover background
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover border
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover text
    }
}