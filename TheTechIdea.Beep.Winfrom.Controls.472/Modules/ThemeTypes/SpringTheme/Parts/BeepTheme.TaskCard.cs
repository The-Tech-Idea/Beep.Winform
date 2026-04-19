using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Task Card Fonts & Colors
        public TypographyStyle TaskCardTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle TaskCardSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle TaskCardUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color TaskCardBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color TaskCardForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color TaskCardTitleForeColor { get; set; } = Color.FromArgb(25, 25, 112);
        public Color TaskCardTitleBackColor { get; set; } =Color.FromArgb(144, 238, 144);
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
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
        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color TaskCardSubTitleBackColor { get; set; } =Color.FromArgb(144, 238, 144);
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
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
        public Color TaskCardMetricTextForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color TaskCardMetricTextBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
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
        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
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
    }
}