using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Task Card Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle TaskCardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for card background
        public Color TaskCardForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for card text
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for card border
        public Color TaskCardTitleForeColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for title
        public Color TaskCardTitleBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for title background
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for subtitle
        public Color TaskCardSubTitleBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for subtitle background
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardMetricTextForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for metric text
        public Color TaskCardMetricTextBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for metric background
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for metric border
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover text
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover background
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover border
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardProgressValueForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for progress value text
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for progress value background
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for progress value border
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}