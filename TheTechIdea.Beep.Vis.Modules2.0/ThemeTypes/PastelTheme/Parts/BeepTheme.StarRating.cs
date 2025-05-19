using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Star Rating Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for text
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for background
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for border
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(255, 220, 200); // Soft peach for filled stars
        public Color StarRatingHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover background
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
        public Color StarRatingSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected text
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected background
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for selected border
        public TypographyStyle StarTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StarSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StarSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StarUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color StarTitleForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for title text
        public Color StarTitleBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for title background
    }
}