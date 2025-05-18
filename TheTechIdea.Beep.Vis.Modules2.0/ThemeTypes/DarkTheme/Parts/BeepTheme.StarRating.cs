using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.LightGray;
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color StarRatingBorderColor { get; set; } = Color.DimGray;
        public Color StarRatingFillColor { get; set; } = Color.Gold;
        public Color StarRatingHoverForeColor { get; set; } = Color.White;
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color StarRatingHoverBorderColor { get; set; } = Color.Gold;
        public Color StarRatingSelectedForeColor { get; set; } = Color.Gold;
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.Gold;

        public Font StarTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font StarSubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Italic);
        public Font StarSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font StarUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color StarTitleForeColor { get; set; } = Color.White;
        public Color StarTitleBackColor { get; set; } = Color.Transparent;
    }
}
