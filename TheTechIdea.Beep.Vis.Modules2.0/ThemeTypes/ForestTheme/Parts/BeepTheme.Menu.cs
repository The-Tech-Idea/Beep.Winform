using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public TypographyStyle MenuItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color MenuBackColor { get; set; } = Color.FromArgb(34, 49, 34); // Dark forest green
        public Color MenuForeColor { get; set; } = Color.FromArgb(200, 230, 201); // Light greenish
        public Color MenuBorderColor { get; set; } = Color.FromArgb(56, 142, 60); // Medium green border
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(220, 237, 200);
        public Color MenuMainItemHoverForeColor { get; set; } = Color.White;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(46, 125, 50);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(27, 94, 32);
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(200, 230, 201);
        public Color MenuItemHoverForeColor { get; set; } = Color.White;
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(56, 142, 60);
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(27, 94, 32);
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(56, 142, 60);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(27, 94, 32);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(46, 125, 50);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
