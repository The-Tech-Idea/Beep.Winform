using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Star Rating Fonts & Colors
//<<<<<<< HEAD
        public Color StarRatingForeColor { get; set; } = Color.LightGray;
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(30, 30, 40);
        public Color StarRatingBorderColor { get; set; } = Color.DarkGray;
        public Color StarRatingFillColor { get; set; } = Color.Gold;
        public Color StarRatingHoverForeColor { get; set; } = Color.White;
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(50, 50, 60);
        public Color StarRatingHoverBorderColor { get; set; } = Color.Yellow;
        public Color StarRatingSelectedForeColor { get; set; } = Color.Yellow;
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(40, 40, 50);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.Orange;

        public TypographyStyle  StarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  StarSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle  StarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  StarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color StarTitleForeColor { get; set; } = Color.WhiteSmoke;
        public Color StarTitleBackColor { get; set; } =Color.FromArgb(20, 24, 30);
    }
}
