using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle ListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle ListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color ListBackColor { get; set; } = Color.White;
        public Color ListForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color ListBorderColor { get; set; } = Color.FromArgb(224, 224, 224);
        public Color ListItemForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color ListItemHoverForeColor { get; set; } = Color.White;
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(224, 224, 224);
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(0, 120, 215);
    }
}
