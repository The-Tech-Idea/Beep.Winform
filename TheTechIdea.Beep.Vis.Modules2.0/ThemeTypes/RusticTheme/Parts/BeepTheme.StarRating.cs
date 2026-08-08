using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color StarRatingHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color StarRatingSelectedForeColor { get; set; } = Color.White;
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public TypographyStyle StarTitleFont { get; set; }
        public TypographyStyle StarSubTitleFont { get; set; }
        public TypographyStyle StarSelectedFont { get; set; }
        public TypographyStyle StarUnSelectedFont { get; set; }
        public Color StarTitleForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color StarTitleBackColor { get; set; } = Color.FromArgb(245, 245, 220);
    }
}
