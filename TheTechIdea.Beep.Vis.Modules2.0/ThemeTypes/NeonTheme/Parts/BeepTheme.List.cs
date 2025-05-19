using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // List Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle ListTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ListSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ListUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color ListBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for list background
        public Color ListForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for list text
        public Color ListBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for list border
        public Color ListItemForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for item text
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover background
        public Color ListItemSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected item text
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected item background
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected item border
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for item border
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover border
    }
}