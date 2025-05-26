using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(255, 215, 0);
        public Color StarRatingHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color StarRatingSelectedForeColor { get; set; } = Color.White;
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public TypographyStyle StarTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StarSubTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StarSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StarUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color StarTitleForeColor { get; set; } = Color.FromArgb(25, 25, 112);
        public Color StarTitleBackColor { get; set; } =Color.FromArgb(144, 238, 144);
    }
}