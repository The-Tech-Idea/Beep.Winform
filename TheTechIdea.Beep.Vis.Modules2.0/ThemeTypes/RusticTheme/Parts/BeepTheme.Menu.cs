using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; }
        public TypographyStyle MenuItemSelectedFont { get; set; }
        public TypographyStyle MenuItemUnSelectedFont { get; set; }
        public Color MenuBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color MenuForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color MenuBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color MenuMainItemHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color MenuItemHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(210, 180, 140);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(238, 232, 205);
        public LinearGradientMode MenuGradiantDirection { get; set; }
    }
}
