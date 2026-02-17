using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle TabHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle TabSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        public Color TabBackColor { get; set; } = Color.White;
        public Color TabForeColor { get; set; } = Color.DimGray;
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(0, 120, 215);  // blue
        public Color ActiveTabForeColor { get; set; } = Color.White;
        public Color InactiveTabBackColor { get; set; } = Color.LightGray;
        public Color InactiveTabForeColor { get; set; } = Color.Gray;

        public Color TabBorderColor { get; set; } = Color.LightGray;
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color TabHoverForeColor { get; set; } = Color.Black;
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color TabHoverBorderColor { get; set; } = Color.Gray;
    }
}
