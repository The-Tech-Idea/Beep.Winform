using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // List Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  ListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public TypographyStyle  ListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
        public TypographyStyle  ListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.White;
        public Color ListForeColor { get; set; } = Color.Black;
        public Color ListBorderColor { get; set; } = Color.LightGray;

        public Color ListItemForeColor { get; set; } = Color.Black;
        public Color ListItemHoverForeColor { get; set; } = Color.DarkBlue;
        public Color ListItemHoverBackColor { get; set; } = Color.LightBlue;

        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color ListItemSelectedBorderColor { get; set; } = Color.DodgerBlue;

        public Color ListItemBorderColor { get; set; } = Color.LightGray;
        public Color ListItemHoverBorderColor { get; set; } = Color.DodgerBlue;
    }
}
