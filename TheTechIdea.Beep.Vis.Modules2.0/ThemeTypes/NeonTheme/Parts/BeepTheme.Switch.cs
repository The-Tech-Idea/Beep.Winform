using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Switch control Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle SwitchTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SwitchSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
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
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SwitchBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for switch background
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for border
        public Color SwitchForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for switch text
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected background
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected border
        public Color SwitchSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected text
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover background
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover border
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
    }
}