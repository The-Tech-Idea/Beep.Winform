using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle  MenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16, FontStyle.Bold);
        public TypographyStyle  MenuItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  MenuItemUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Regular);
        public Color MenuBackColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color MenuForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color MenuBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color MenuMainItemHoverForeColor { get; set; } = Color.White;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color MenuItemHoverForeColor { get; set; } = Color.White;
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(220, 230, 240);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(180, 200, 220);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(200, 215, 230);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
