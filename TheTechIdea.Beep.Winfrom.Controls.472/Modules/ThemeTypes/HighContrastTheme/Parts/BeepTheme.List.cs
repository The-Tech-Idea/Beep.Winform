using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // List Fonts & Colors
        public TypographyStyle  ListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  ListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  ListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.Black;
        public Color ListForeColor { get; set; } = Color.White;
        public Color ListBorderColor { get; set; } = Color.White;
        public Color ListItemForeColor { get; set; } = Color.White;
        public Color ListItemHoverForeColor { get; set; } = Color.Black;
        public Color ListItemHoverBackColor { get; set; } = Color.Yellow;
        public Color ListItemSelectedForeColor { get; set; } = Color.Black;
        public Color ListItemSelectedBackColor { get; set; } = Color.Cyan;
        public Color ListItemSelectedBorderColor { get; set; } = Color.White;
        public Color ListItemBorderColor { get; set; } = Color.White;
        public Color ListItemHoverBorderColor { get; set; } = Color.Yellow;
    }
}
