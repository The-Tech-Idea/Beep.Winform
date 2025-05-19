using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color StarRatingHoverForeColor { get; set; } = Color.White;
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color StarRatingSelectedForeColor { get; set; } = Color.White;
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public TypographyStyle StarTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StarSubTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StarSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StarUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color StarTitleForeColor { get; set; } = Color.White;
        public Color StarTitleBackColor { get; set; } = Color.Transparent;
    }
}