using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle  MenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16f, FontStyle.Bold);
        public TypographyStyle  MenuItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  MenuItemUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Regular);

        public Color MenuBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color MenuForeColor { get; set; } = Color.White;
        public Color MenuBorderColor { get; set; } = Color.FromArgb(60, 60, 60);

        public Color MenuMainItemForeColor { get; set; } = Color.WhiteSmoke;
        public Color MenuMainItemHoverForeColor { get; set; } = Color.CornflowerBlue;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(40, 40, 70);

        public Color MenuItemForeColor { get; set; } = Color.LightGray;
        public Color MenuItemHoverForeColor { get; set; } = Color.CornflowerBlue;
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(55, 55, 55);
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(60, 60, 90);

        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(35, 35, 35);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(15, 15, 15);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(25, 25, 25);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
