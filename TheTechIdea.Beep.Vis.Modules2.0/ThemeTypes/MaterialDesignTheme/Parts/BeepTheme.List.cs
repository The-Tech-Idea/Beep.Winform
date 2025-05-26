using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // List Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  ListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 16f, FontStyle.Bold);
        public TypographyStyle  ListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Regular);
        public TypographyStyle  ListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.White;
        public Color ListForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900
        public Color ListBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300

        public Color ListItemForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(232, 240, 254); // Light Blue 50

        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(21, 101, 192); // Blue 800

        public Color ListItemBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
    }
}
