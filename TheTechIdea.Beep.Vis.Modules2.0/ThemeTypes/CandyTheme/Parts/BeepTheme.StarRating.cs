using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Star Rating Fonts & Colors

        // Unselected/idle state: pastel pink star outlines on lemon yellow background
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(255, 224, 235);      // Pastel Pink
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(255, 253, 194);      // Lemon Yellow
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(127, 255, 212);    // Mint

        // Star fill (when rated/partially rated): candy pink!
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(240, 100, 180);      // Candy Pink

        // Hover: mint stars with pastel blue background
        public Color StarRatingHoverForeColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        // Selected: strong candy pink stars on mint, candy border
        public Color StarRatingSelectedForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(255, 223, 93); // Lemon

        // Fonts
        public TypographyStyle StarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 12f, FontStyle.Bold);
        public TypographyStyle StarSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Italic);
        public TypographyStyle StarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
        public TypographyStyle StarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);

        // Title/Subtitle color backgrounds
        public Color StarTitleForeColor { get; set; } = Color.FromArgb(240, 100, 180);    // Candy Pink
        public Color StarTitleBackColor { get; set; } = Color.FromArgb(255, 224, 235);    // Pastel Pink
    }
}
