using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color StarRatingHoverForeColor { get; set; } = Color.White;
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public Color StarRatingSelectedForeColor { get; set; } = Color.White;
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(192, 128, 0);
        public TypographyStyle StarTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StarSubTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StarSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StarUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color StarTitleForeColor { get; set; } = Color.White;
        public Color StarTitleBackColor { get; set; } =Color.FromArgb(96, 96, 96);
    }
}