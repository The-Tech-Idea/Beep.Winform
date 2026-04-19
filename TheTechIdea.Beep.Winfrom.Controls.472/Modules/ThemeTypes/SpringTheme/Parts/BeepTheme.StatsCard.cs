using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StatsSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
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
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color StatsCardForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color StatsCardTitleForeColor { get; set; } = Color.FromArgb(25, 25, 112);
        public Color StatsCardTitleBackColor { get; set; } =Color.FromArgb(144, 238, 144);
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color StatsCardSubTitleBackColor { get; set; } =Color.FromArgb(144, 238, 144);
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(70, 70, 70),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color StatsCardValueHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(100, 100, 100);
        public Color StatsCardInfoBackColor { get; set; } =Color.FromArgb(144, 238, 144);
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.1f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(100, 100, 100),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color StatsCardTrendBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(60, 179, 113),
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}