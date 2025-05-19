using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle TabHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle TabSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);

        public Color TabBackColor { get; set; } = Color.Black;
        public Color TabForeColor { get; set; } = Color.LightGray;

        public Color ActiveTabBackColor { get; set; } = Color.WhiteSmoke;
        public Color ActiveTabForeColor { get; set; } = Color.Black;

        public Color InactiveTabBackColor { get; set; } = Color.Black;
        public Color InactiveTabForeColor { get; set; } = Color.Gray;

        public Color TabBorderColor { get; set; } = Color.Gray;

        public Color TabHoverBackColor { get; set; } = Color.DimGray;
        public Color TabHoverForeColor { get; set; } = Color.White;

        public Color TabSelectedBackColor { get; set; } = Color.WhiteSmoke;
        public Color TabSelectedForeColor { get; set; } = Color.Black;

        public Color TabSelectedBorderColor { get; set; } = Color.DarkGray;
        public Color TabHoverBorderColor { get; set; } = Color.Silver;
    }
}
