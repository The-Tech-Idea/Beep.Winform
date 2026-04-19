using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // List Fonts & Colors
        public TypographyStyle  ListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  ListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  ListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color ListBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color ListForeColor { get; set; } = Color.WhiteSmoke;
        public Color ListBorderColor { get; set; } = Color.Gray;
        public Color ListItemForeColor { get; set; } = Color.WhiteSmoke;
        public Color ListItemHoverForeColor { get; set; } = Color.LightSkyBlue;
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color ListItemSelectedBorderColor { get; set; } = Color.CornflowerBlue;
        public Color ListItemBorderColor { get; set; } = Color.Gray;
        public Color ListItemHoverBorderColor { get; set; } = Color.LightSkyBlue;
    }
}
