using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StatsSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StatsUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color StatsCardForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color StatsCardTitleForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color StatsCardTitleBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(90, 45, 0);
        public Color StatsCardSubTitleBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(90, 45, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(240, 235, 215);
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color StatsCardValueHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(120, 60, 0);
        public Color StatsCardInfoBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(200, 180, 160);
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.1f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(120, 60, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color StatsCardTrendBackColor { get; set; } = Color.FromArgb(240, 235, 215);
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(160, 82, 45),
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}