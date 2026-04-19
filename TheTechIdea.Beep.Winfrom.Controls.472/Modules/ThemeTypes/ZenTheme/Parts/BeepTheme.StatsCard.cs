using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(34, 34, 34),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StatsSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StatsUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color StatsCardForeColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color StatsCardTitleForeColor { get; set; } = Color.White;
        public Color StatsCardTitleBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color StatsCardSubTitleBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color StatsCardValueHoverForeColor { get; set; } = Color.White;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(96, 195, 100);
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(76, 175, 80),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color StatsCardInfoBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color StatsCardTrendBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(76, 175, 80),
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}