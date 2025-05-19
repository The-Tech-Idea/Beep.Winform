using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Stats Card Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle StatsTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StatsSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StatsUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color StatsCardBackColor { get; set; } = Color.FromArgb(255, 255, 255); // White for card background
        public Color StatsCardForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for card text
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for card border
        public Color StatsCardTitleForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for title
        public Color StatsCardTitleBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for title background
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
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
        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(100, 100, 100); // Medium gray for subtitle
        public Color StatsCardSubTitleBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for subtitle background
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
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
        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for value text
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for value background
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for value border
        public Color StatsCardValueHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover background
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(100, 100, 100); // Medium gray for info text
        public Color StatsCardInfoBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for info background
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for info border
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(255, 220, 200); // Soft peach for trend text
        public Color StatsCardTrendBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for trend background
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(255, 220, 200); // Soft peach for trend border
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(255, 220, 200), // Soft peach
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}