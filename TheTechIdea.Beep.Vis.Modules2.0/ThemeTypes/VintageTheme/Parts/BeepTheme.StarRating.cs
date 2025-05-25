using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color StarRatingHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color StarRatingSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public TypographyStyle StarTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StarSubTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StarSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StarUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color StarTitleForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color StarTitleBackColor { get; set; } = Color.Transparent;
    }
}