using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Star Rating Fonts & Colors.
        // The old palette had Material blue (33,150,243) pasted into every *BackColor slot -
        // empty stars rendered BLUE and looked selected; SelectedFore was Goldenrod, i.e.
        // gold ink on the gold fill (unreadable chip numbers in NumericScale).
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(68, 68, 68);        // label ink beside stars
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(205, 205, 205);     // EMPTY star - muted neutral
        public Color StarRatingBorderColor { get; set; } = Color.DarkGoldenrod;
        public Color StarRatingFillColor { get; set; } = Color.Gold;
        public Color StarRatingHoverForeColor { get; set; } = Color.Orange;
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(255, 243, 205); // light gold tint
        public Color StarRatingHoverBorderColor { get; set; } = Color.OrangeRed;
        public Color StarRatingSelectedForeColor { get; set; } = Color.FromArgb(66, 50, 0);  // dark ink ON the gold fill
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(255, 236, 170);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.DarkGoldenrod;

        public TypographyStyle StarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle StarSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);
        public TypographyStyle StarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle StarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color StarTitleForeColor { get; set; } = Color.Black;
        public Color StarTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
    }
}
