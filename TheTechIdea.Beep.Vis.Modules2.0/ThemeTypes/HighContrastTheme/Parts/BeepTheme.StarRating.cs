using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.White;
        public Color StarRatingBackColor { get; set; } = Color.Black;
        public Color StarRatingBorderColor { get; set; } = Color.White;
        public Color StarRatingFillColor { get; set; } = Color.Yellow;
        public Color StarRatingHoverForeColor { get; set; } = Color.Yellow;
        public Color StarRatingHoverBackColor { get; set; } = Color.DarkSlateGray;
        public Color StarRatingHoverBorderColor { get; set; } = Color.Yellow;
        public Color StarRatingSelectedForeColor { get; set; } = Color.Black;
        public Color StarRatingSelectedBackColor { get; set; } = Color.Yellow;
        public Color StarRatingSelectedBorderColor { get; set; } = Color.White;
        public Font StarTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font StarSubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Italic);
        public Font StarSelectedFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Bold);
        public Font StarUnSelectedFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);
        public Color StarTitleForeColor { get; set; } = Color.White;
        public Color StarTitleBackColor { get; set; } = Color.Black;
    }
}
