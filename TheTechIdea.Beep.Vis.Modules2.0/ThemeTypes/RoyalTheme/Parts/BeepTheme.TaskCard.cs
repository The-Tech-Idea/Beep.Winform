using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Task Card Fonts & Colors
        public TypographyStyle TaskCardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TaskCardSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TaskCardUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color TaskCardForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color TaskCardTitleForeColor { get; set; } = Color.White;
        public Color TaskCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color TaskCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardMetricTextForeColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color TaskCardMetricTextBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.White;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 193, 7),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardProgressValueForeColor { get; set; } = Color.FromArgb(75, 192, 192);
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(75, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}