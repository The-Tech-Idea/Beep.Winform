using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Task Card Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle TaskCardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TaskCardSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle TaskCardUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color TaskCardBackColor { get; set; } = Color.FromArgb(255, 255, 255); // White for card background
        public Color TaskCardForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for card text
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for card border
        public Color TaskCardTitleForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for title
        public Color TaskCardTitleBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for title background
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(100, 100, 100); // Medium gray for subtitle
        public Color TaskCardSubTitleBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for subtitle background
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardMetricTextForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for metric text
        public Color TaskCardMetricTextBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for metric background
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for metric border
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover background
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardProgressValueForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for progress value text
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for progress value background
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for progress value border
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}