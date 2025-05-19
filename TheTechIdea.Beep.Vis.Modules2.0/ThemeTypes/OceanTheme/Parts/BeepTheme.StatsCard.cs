using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Stats Card Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle StatsTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StatsSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StatsUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color StatsCardBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for card background
        public Color StatsCardForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for card text
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for card border
        public Color StatsCardTitleForeColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for title
        public Color StatsCardTitleBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for title background
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
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
        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for subtitle
        public Color StatsCardSubTitleBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for subtitle background
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
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
        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for value text
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for value background
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for value border
        public Color StatsCardValueHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover text
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover background
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover border
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(50, 120, 160); // Deep sky blue for info text
        public Color StatsCardInfoBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for info background
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(50, 120, 160); // Deep sky blue for info border
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(50, 120, 160), // Deep sky blue
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(255, 180, 90); // Soft orange for trend text
        public Color StatsCardTrendBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for trend background
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(255, 180, 90); // Soft orange for trend border
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(255, 180, 90), // Soft orange
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}