using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Tab Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle TabFont { get; set; } = new TypographyStyle
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
        public TypographyStyle TabHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(241, 196, 15), // Neon yellow
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
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TabBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for tab background
        public Color TabForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for tab text
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for active tab
        public Color ActiveTabForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for active tab text
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for inactive tab
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for inactive tab text
        public Color TabBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for tab border
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected tab
        public Color TabSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected tab text
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected border
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover border
    }
}