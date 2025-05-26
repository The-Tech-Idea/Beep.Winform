using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
//<<<<<<< HEAD
        // Star Rating Fonts & Colors with Material Design defaults
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Color StarRatingBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Grey 400
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(255, 193, 7); // Amber 500 (for fill)
        public Color StarRatingHoverForeColor { get; set; } = Color.FromArgb(255, 213, 79); // Amber 400
        public Color StarRatingHoverBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(255, 213, 79); // Amber 400
        public Color StarRatingSelectedForeColor { get; set; } = Color.FromArgb(255, 193, 7); // Amber 500
        public Color StarRatingSelectedBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7); // Amber 500

        public TypographyStyle  StarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 16f, FontStyle.Bold);
        public TypographyStyle  StarSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 12f, FontStyle.Regular);
        public TypographyStyle  StarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Bold);
        public TypographyStyle  StarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Regular);

        public Color StarTitleForeColor { get; set; } = Color.Black;
        public Color StarTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
    }
}
