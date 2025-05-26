using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // List Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  ListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle  ListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  ListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color ListForeColor { get; set; } = Color.White;
        public Color ListBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Subtle border

        public Color ListItemForeColor { get; set; } = Color.White;
        public Color ListItemHoverForeColor { get; set; } = Color.White;
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Hover dark blue

        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color ListItemSelectedBorderColor { get; set; } = Color.White;

        public Color ListItemBorderColor { get; set; } = Color.FromArgb(0x44, 0x44, 0x44); // Divider tone
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Light blue hover
    }
}
