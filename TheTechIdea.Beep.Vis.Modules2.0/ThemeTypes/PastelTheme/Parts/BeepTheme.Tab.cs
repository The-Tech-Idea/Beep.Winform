using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Tab Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle TabFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
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
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
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
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TabBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for tab background
        public Color TabForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for tab text
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for active tab
        public Color ActiveTabForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for active tab text
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for inactive tab
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for inactive tab text
        public Color TabBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for tab border
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected tab
        public Color TabSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected tab text
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for selected border
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
    }
}