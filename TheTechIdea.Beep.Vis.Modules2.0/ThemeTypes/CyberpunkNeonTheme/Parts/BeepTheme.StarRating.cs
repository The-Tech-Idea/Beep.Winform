using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Star Rating Fonts & Colors

        public Color StarRatingForeColor { get; set; } = Color.FromArgb(0, 255, 255);          // Neon Cyan (star border/default)
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(18, 18, 32);           // Cyberpunk Black (panel)
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(255, 0, 255);        // Neon Magenta (star outline)
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(255, 255, 0);          // Neon Yellow (filled star)

        public Color StarRatingHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);     // Neon Yellow (hover)
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(0, 255, 128);     // Neon Green (hover BG)
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(0, 255, 255);   // Neon Cyan (hover outline)

        public Color StarRatingSelectedForeColor { get; set; } = Color.FromArgb(255, 0, 255);  // Neon Magenta (selected star)
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(34, 34, 68);   // Deep panel
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(0, 255, 128);// Neon Green

        public TypographyStyle StarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14f, FontStyle.Bold);
        public TypographyStyle StarSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Italic);
        public TypographyStyle StarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle StarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Regular);

        public Color StarTitleForeColor { get; set; } = Color.FromArgb(0, 255, 255);           // Neon Cyan
        public Color StarTitleBackColor { get; set; } = Color.FromArgb(18, 18, 32);
    }
}
