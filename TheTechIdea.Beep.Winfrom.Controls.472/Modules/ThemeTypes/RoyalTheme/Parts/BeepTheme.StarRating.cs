using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color StarRatingHoverForeColor { get; set; } = Color.White;
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color StarRatingSelectedForeColor { get; set; } = Color.White;
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public TypographyStyle StarTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StarSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StarSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StarUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StarTitleForeColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color StarTitleBackColor { get; set; } =Color.FromArgb(70, 70, 130);
    }
}