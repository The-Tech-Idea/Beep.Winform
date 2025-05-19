using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Tab Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle TabFont { get; set; } = new TypographyStyle
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
        public TypographyStyle TabHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(90, 180, 90), // Soft green
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TabBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for tab background
        public Color TabForeColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for tab text
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for active tab
        public Color ActiveTabForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for active tab text
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(220, 220, 225); // Slightly darker gray for inactive tab
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for inactive tab text
        public Color TabBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for tab border
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for hover
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover text
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected tab
        public Color TabSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for selected tab text
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected border
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover border
    }
}