using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle ListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle ListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.FromArgb(245, 240, 230); // Light sand
        public Color ListForeColor { get; set; } = Color.FromArgb(101, 67, 33);    // Dark brown
        public Color ListBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan

        public Color ListItemForeColor { get; set; } = Color.FromArgb(101, 67, 33); // Dark brown
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(80, 50, 20); // Darker brown
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(255, 228, 181); // Moccasin light orange
        public Color ListItemSelectedForeColor { get; set; } = Color.FromArgb(244, 164, 96); // Warm orange
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna brown

        public Color ListItemBorderColor { get; set; } = Color.FromArgb(222, 184, 135); // BurlyWood
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
    }
}
