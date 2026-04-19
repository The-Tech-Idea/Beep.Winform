using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.Gold;
        public Color StarRatingBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color StarRatingBorderColor { get; set; } = Color.DarkGoldenrod;
        public Color StarRatingFillColor { get; set; } = Color.Gold;
        public Color StarRatingHoverForeColor { get; set; } = Color.Orange;
        public Color StarRatingHoverBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color StarRatingHoverBorderColor { get; set; } = Color.OrangeRed;
        public Color StarRatingSelectedForeColor { get; set; } = Color.Goldenrod;
        public Color StarRatingSelectedBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.DarkGoldenrod;

        public TypographyStyle StarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle StarSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);
        public TypographyStyle StarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle StarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color StarTitleForeColor { get; set; } = Color.Black;
        public Color StarTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
    }
}
