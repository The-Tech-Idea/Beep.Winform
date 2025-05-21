using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color StarRatingHoverForeColor { get; set; } = Color.White;
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color StarRatingSelectedForeColor { get; set; } = Color.White;
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public TypographyStyle StarTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle StarSubTitleFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public TypographyStyle StarSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle StarUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color StarTitleForeColor { get; set; } = Color.White;
        public Color StarTitleBackColor { get; set; } = Color.Transparent;
    }
}