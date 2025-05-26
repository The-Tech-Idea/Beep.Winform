using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.LightGray;
        public Color StarRatingBackColor { get; set; } = Color.DimGray;
        public Color StarRatingBorderColor { get; set; } = Color.Gray;
        public Color StarRatingFillColor { get; set; } = Color.WhiteSmoke;
        public Color StarRatingHoverForeColor { get; set; } = Color.Gainsboro;
        public Color StarRatingHoverBackColor { get; set; } = Color.DimGray;
        public Color StarRatingHoverBorderColor { get; set; } = Color.Silver;
        public Color StarRatingSelectedForeColor { get; set; } = Color.White;
        public Color StarRatingSelectedBackColor { get; set; } = Color.DimGray;
        public Color StarRatingSelectedBorderColor { get; set; } = Color.White;

        public TypographyStyle StarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle StarSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);
        public TypographyStyle StarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle StarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);

        public Color StarTitleForeColor { get; set; } = Color.WhiteSmoke;
        public Color StarTitleBackColor { get; set; } = Color.DimGray;
    }
}
