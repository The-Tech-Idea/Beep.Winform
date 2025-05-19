using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // List Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle ListTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ListSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ListUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color ListBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for list background
        public Color ListForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for list text
        public Color ListBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for list border
        public Color ListItemForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for item text
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover background
        public Color ListItemSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected item text
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected item background
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for selected item border
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for item border
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
    }
}