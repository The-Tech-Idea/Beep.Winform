using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle MenuItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color MenuBackColor { get; set; } = Color.White;
        public Color MenuForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color MenuBorderColor { get; set; } = Color.LightGray;
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color MenuMainItemHoverForeColor { get; set; } = Color.White;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(55, 55, 55);
        public Color MenuItemHoverForeColor { get; set; } = Color.White;
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color MenuGradiantStartColor { get; set; } = Color.White;
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(230, 230, 230);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
