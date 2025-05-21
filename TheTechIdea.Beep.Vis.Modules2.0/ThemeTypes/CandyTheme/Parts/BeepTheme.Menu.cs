using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Menu Fonts & Colors

        public TypographyStyle MenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 12f, FontStyle.Bold);
        public TypographyStyle MenuItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);

        public Color MenuBackColor { get; set; } = Color.FromArgb(255, 224, 235);           // Pastel Pink
        public Color MenuForeColor { get; set; } = Color.FromArgb(44, 62, 80);              // Navy
        public Color MenuBorderColor { get; set; } = Color.FromArgb(127, 255, 212);         // Mint

        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(44, 62, 80);      // Navy
        public Color MenuMainItemHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue

        public Color MenuMainItemSelectedForeColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        public Color MenuItemForeColor { get; set; } = Color.FromArgb(44, 62, 80);          // Navy
        public Color MenuItemHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180);  // Candy Pink
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(204, 255, 240);  // Mint

        public Color MenuItemSelectedForeColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        // Menu gradient: pastel pink -> mint -> lemon
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(255, 224, 235);  // Pastel Pink
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(204, 255, 240);    // Mint
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
