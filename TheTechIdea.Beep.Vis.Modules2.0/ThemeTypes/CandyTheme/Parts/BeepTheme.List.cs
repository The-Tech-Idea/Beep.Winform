using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // List Fonts & Colors

        public TypographyStyle ListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 12f, FontStyle.Bold);
        public TypographyStyle ListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
        public TypographyStyle ListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.FromArgb(255, 224, 235);        // Pastel Pink
        public Color ListForeColor { get; set; } = Color.FromArgb(44, 62, 80);           // Navy

        public Color ListBorderColor { get; set; } = Color.FromArgb(127, 255, 212);      // Mint

        public Color ListItemForeColor { get; set; } = Color.FromArgb(44, 62, 80);       // Navy
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue

        public Color ListItemSelectedForeColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(54, 162, 235); // Pastel Blue

        public Color ListItemBorderColor { get; set; } = Color.FromArgb(206, 183, 255);      // Pastel Lavender
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(255, 223, 93);  // Lemon (for pop on hover)
    }
}
