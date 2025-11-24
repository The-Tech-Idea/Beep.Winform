using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.Goldenrod;
        public Color StarRatingBackColor { get; set; } =Color.White;
        public Color StarRatingBorderColor { get; set; } = Color.DarkGoldenrod;
        public Color StarRatingFillColor { get; set; } = Color.Gold;
        public Color StarRatingHoverForeColor { get; set; } = Color.Orange;
        public Color StarRatingHoverBackColor { get; set; } =Color.White;
        public Color StarRatingHoverBorderColor { get; set; } = Color.OrangeRed;
        public Color StarRatingSelectedForeColor { get; set; } = Color.Gold;
        public Color StarRatingSelectedBackColor { get; set; } =Color.White;
        public Color StarRatingSelectedBorderColor { get; set; } = Color.Goldenrod;
        public TypographyStyle  StarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  StarSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle  StarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  StarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color StarTitleForeColor { get; set; } = Color.Black;
        public Color StarTitleBackColor { get; set; } =Color.White;
    }
}
