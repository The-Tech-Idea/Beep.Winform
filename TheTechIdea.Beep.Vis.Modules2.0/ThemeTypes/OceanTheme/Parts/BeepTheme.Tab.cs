using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Tab Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle TabFont { get; set; } = new TypographyStyle
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
        public TypographyStyle TabHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TabSelectedFont { get; set; } = new TypographyStyle
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
        public Color TabBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for tab background
        public Color TabForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for tab text
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for active tab
        public Color ActiveTabForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for active tab text
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for inactive tab
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for inactive tab text
        public Color TabBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for tab border
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover text
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected tab
        public Color TabSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected tab text
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected border
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover border
    }
}