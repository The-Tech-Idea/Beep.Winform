using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color StarRatingHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color StarRatingSelectedForeColor { get; set; } = Color.White;
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
        public TypographyStyle StarTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle StarSubTitleFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public TypographyStyle StarSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle StarUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color StarTitleForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color StarTitleBackColor { get; set; } = Color.Transparent;
    }
}