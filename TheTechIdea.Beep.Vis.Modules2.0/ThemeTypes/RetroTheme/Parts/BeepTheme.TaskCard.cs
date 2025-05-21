using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Task Card Fonts & Colors
        public TypographyStyle TaskCardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TaskCardSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TaskCardUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color TaskCardForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color TaskCardTitleForeColor { get; set; } = Color.White;
        public Color TaskCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color TaskCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardMetricTextForeColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color TaskCardMetricTextBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.White;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 165, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TaskCardProgressValueForeColor { get; set; } = Color.FromArgb(64, 255, 64);
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(64, 255, 64),
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}