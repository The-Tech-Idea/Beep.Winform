using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Star Rating Fonts & Colors
//<<<<<<< HEAD
        public Color StarRatingForeColor { get; set; } = Color.Gold;
        public Color StarRatingBackColor { get; set; } = Color.White;
        public Color StarRatingBorderColor { get; set; } = Color.Gray;
        public Color StarRatingFillColor { get; set; } = Color.Orange;
        public Color StarRatingHoverForeColor { get; set; } = Color.OrangeRed;
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(255, 245, 230);
        public Color StarRatingHoverBorderColor { get; set; } = Color.DarkOrange;
        public Color StarRatingSelectedForeColor { get; set; } = Color.White;
        public Color StarRatingSelectedBackColor { get; set; } = Color.Orange;
        public Color StarRatingSelectedBorderColor { get; set; } = Color.DarkOrange;

        public TypographyStyle  StarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  StarSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Italic);
        public TypographyStyle  StarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  StarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color StarTitleForeColor { get; set; } = Color.Black;
        public Color StarTitleBackColor { get; set; } = Color.WhiteSmoke;
    }
}
