using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(245, 222, 179); // Wheat
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color StarRatingHoverForeColor { get; set; } = Color.FromArgb(222, 184, 135); // Burlywood
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(255, 239, 213); // PapayaWhip
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
        public Color StarRatingSelectedForeColor { get; set; } = Color.FromArgb(255, 165, 0); // Orange
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(255, 250, 205); // LemonChiffon
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(255, 140, 0); // DarkOrange

        public TypographyStyle StarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle StarSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Italic);
        public TypographyStyle StarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle StarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color StarTitleForeColor { get; set; } = Color.FromArgb(101, 67, 33); // Dark Brown
        public Color StarTitleBackColor { get; set; } = Color.FromArgb(255, 248, 220); // Cornsilk
    }
}
