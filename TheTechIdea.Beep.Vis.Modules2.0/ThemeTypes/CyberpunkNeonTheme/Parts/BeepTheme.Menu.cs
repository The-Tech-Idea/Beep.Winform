using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Menu Fonts & Colors

        public TypographyStyle MenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 13f, FontStyle.Bold);
        public TypographyStyle MenuItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Bold);
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Regular);

        public Color MenuBackColor { get; set; } = Color.FromArgb(18, 18, 32);                      // Cyberpunk Black
        public Color MenuForeColor { get; set; } = Color.FromArgb(0, 255, 255);                     // Neon Cyan
        public Color MenuBorderColor { get; set; } = Color.FromArgb(255, 0, 255);                   // Neon Magenta

        // Main item states
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(0, 255, 255);             // Neon Cyan
        public Color MenuMainItemHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);        // Neon Yellow
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(0, 255, 128);        // Neon Green
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(255, 0, 255);     // Neon Magenta

        // Item states
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(0, 255, 255);                 // Neon Cyan
        public Color MenuItemHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);            // Neon Yellow
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(34, 34, 68);             // Cyberpunk Panel
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(255, 0, 255);         // Neon Magenta

        // Gradient bar for menus (e.g., side bar, title)
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(255, 0, 255);            // Neon Magenta
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(0, 255, 255);              // Neon Cyan
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(0, 255, 128);           // Neon Green
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}
