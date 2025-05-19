using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public TypographyStyle MenuItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);

        public Color MenuBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color MenuForeColor { get; set; } = Color.LightGray;
        public Color MenuBorderColor { get; set; } = Color.FromArgb(60, 60, 60);

        public Color MenuMainItemForeColor { get; set; } = Color.WhiteSmoke;
        public Color MenuMainItemHoverForeColor { get; set; } = Color.CornflowerBlue;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(50, 50, 70);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(80, 80, 110);

        public Color MenuItemForeColor { get; set; } = Color.LightGray;
        public Color MenuItemHoverForeColor { get; set; } = Color.CornflowerBlue;
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(50, 50, 70);
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(80, 80, 110);

        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(25, 25, 25);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(30, 30, 30);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
