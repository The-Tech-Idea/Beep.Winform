using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle  TabFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  TabHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  TabSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color TabBackColor { get; set; } = Color.LightGray;
        public Color TabForeColor { get; set; } = Color.Black;
        public Color ActiveTabBackColor { get; set; } = Color.DodgerBlue;
        public Color ActiveTabForeColor { get; set; } = Color.White;
        public Color InactiveTabBackColor { get; set; } = Color.Gray;
        public Color InactiveTabForeColor { get; set; } = Color.DarkGray;
        public Color TabBorderColor { get; set; } = Color.Silver;
        public Color TabHoverBackColor { get; set; } = Color.SkyBlue;
        public Color TabHoverForeColor { get; set; } = Color.Black;
        public Color TabSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.DodgerBlue;
        public Color TabHoverBorderColor { get; set; } = Color.SkyBlue;
    }
}
