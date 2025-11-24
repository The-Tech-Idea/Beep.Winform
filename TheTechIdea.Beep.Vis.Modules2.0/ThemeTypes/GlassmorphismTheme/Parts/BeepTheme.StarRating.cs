using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.Gray;
        public Color StarRatingBackColor { get; set; } = Color.WhiteSmoke;
        public Color StarRatingBorderColor { get; set; } = Color.LightGray;
        public Color StarRatingFillColor { get; set; } = Color.Gold;

        public Color StarRatingHoverForeColor { get; set; } = Color.Orange;
        public Color StarRatingHoverBackColor { get; set; } = Color.LightYellow;
        public Color StarRatingHoverBorderColor { get; set; } = Color.DarkOrange;

        public Color StarRatingSelectedForeColor { get; set; } = Color.White;
        public Color StarRatingSelectedBackColor { get; set; } = Color.Goldenrod;
        public Color StarRatingSelectedBorderColor { get; set; } = Color.DarkGoldenrod;

        public TypographyStyle  StarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle  StarSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Italic);
        public TypographyStyle  StarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  StarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);

        public Color StarTitleForeColor { get; set; } = Color.Black;
        public Color StarTitleBackColor { get; set; } = Color.White;
    }
}
