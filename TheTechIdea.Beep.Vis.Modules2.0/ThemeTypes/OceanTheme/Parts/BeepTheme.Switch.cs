using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Switch control Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle SwitchTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SwitchBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for switch background
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for border
        public Color SwitchForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for switch text
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected background
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected border
        public Color SwitchSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected text
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover background
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover border
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover text
    }
}