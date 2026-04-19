using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle ListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle ListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color ListBackColor { get; set; } = Color.FromArgb(34, 49, 34); // Dark green
        public Color ListForeColor { get; set; } = Color.FromArgb(200, 230, 201); // Light green
        public Color ListBorderColor { get; set; } = Color.FromArgb(46, 125, 50); // Medium green
        public Color ListItemForeColor { get; set; } = Color.FromArgb(200, 230, 201);
        public Color ListItemHoverForeColor { get; set; } = Color.White;
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(56, 142, 60); // Highlight green
        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(27, 94, 32); // Darker selected green
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(46, 125, 50);
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(129, 199, 132);
    }
}
