using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.White;
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color StarRatingHoverForeColor { get; set; } = Color.White;
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
        public Color StarRatingSelectedForeColor { get; set; } = Color.Black;
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(203, 75, 22);
        public TypographyStyle StarTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle StarSubTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(147, 161, 161), IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle StarSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle StarUnSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color StarTitleForeColor { get; set; } = Color.White;
        public Color StarTitleBackColor { get; set; } = Color.FromArgb(7, 54, 66);
    }
}