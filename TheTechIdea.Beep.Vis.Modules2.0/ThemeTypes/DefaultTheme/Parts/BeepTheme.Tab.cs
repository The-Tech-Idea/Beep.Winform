using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle TabHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle TabSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public Color TabBackColor { get; set; } = Color.White;
        public Color TabForeColor { get; set; } = Color.DimGray;
        public Color ActiveTabBackColor { get; set; } = Color.WhiteSmoke;
        public Color ActiveTabForeColor { get; set; } = Color.DodgerBlue;
        public Color InactiveTabBackColor { get; set; } = Color.LightGray;
        public Color InactiveTabForeColor { get; set; } = Color.Gray;
        public Color TabBorderColor { get; set; } = Color.Silver;
        public Color TabHoverBackColor { get; set; } = Color.LightBlue;
        public Color TabHoverForeColor { get; set; } = Color.Black;
        public Color TabSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.RoyalBlue;
        public Color TabHoverBorderColor { get; set; } = Color.LightSteelBlue;
    }
}
