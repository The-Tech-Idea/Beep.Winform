using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public TypographyStyle ListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
        public TypographyStyle ListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color ListForeColor { get; set; } = Color.WhiteSmoke;
        public Color ListBorderColor { get; set; } = Color.Gray;

        public Color ListItemForeColor { get; set; } = Color.LightGray;
        public Color ListItemHoverForeColor { get; set; } = Color.White;
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);  // Accent blue
        public Color ListItemSelectedBorderColor { get; set; } = Color.DodgerBlue;

        public Color ListItemBorderColor { get; set; } = Color.DarkGray;
        public Color ListItemHoverBorderColor { get; set; } = Color.LightSkyBlue;
    }
}
