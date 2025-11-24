using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // List Fonts & Colors
        public TypographyStyle  ListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  ListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  ListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color ListBackColor { get; set; } = Color.White;
        public Color ListForeColor { get; set; } = Color.Black;
        public Color ListBorderColor { get; set; } = Color.Gray;
        public Color ListItemForeColor { get; set; } = Color.Black;
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(230, 240, 255);
        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color ListItemBorderColor { get; set; } = Color.LightGray;
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(0, 120, 215);
    }
}
