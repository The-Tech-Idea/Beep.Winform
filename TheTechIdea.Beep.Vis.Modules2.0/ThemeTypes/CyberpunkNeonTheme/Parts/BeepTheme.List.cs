using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // List Fonts & Colors

        public TypographyStyle ListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14f, FontStyle.Bold);
        public TypographyStyle ListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold);
        public TypographyStyle ListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.FromArgb(18, 18, 32);             // Cyberpunk Black
        public Color ListForeColor { get; set; } = Color.FromArgb(0, 255, 255);            // Neon Cyan

        public Color ListBorderColor { get; set; } = Color.FromArgb(255, 0, 255);          // Neon Magenta

        public Color ListItemForeColor { get; set; } = Color.FromArgb(0, 255, 255);        // Neon Cyan
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);   // Neon Yellow
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(34, 34, 68);    // Cyberpunk Panel

        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(255, 0, 255);// Neon Magenta
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(0, 255, 128);// Neon Green

        public Color ListItemBorderColor { get; set; } = Color.FromArgb(0, 255, 255);      // Neon Cyan
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(255, 255, 0); // Neon Yellow
    }
}
