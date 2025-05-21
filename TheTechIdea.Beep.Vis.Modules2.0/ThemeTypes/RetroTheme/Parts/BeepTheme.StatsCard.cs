using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StatsSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StatsUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color StatsCardBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color StatsCardForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color StatsCardTitleForeColor { get; set; } = Color.White;
        public Color StatsCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
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
        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color StatsCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
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
        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color StatsCardValueHoverForeColor { get; set; } = Color.White;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
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
        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color StatsCardInfoBackColor { get; set; } = Color.Transparent;
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
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
        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(64, 255, 64);
        public Color StatsCardTrendBackColor { get; set; } = Color.Transparent;
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
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