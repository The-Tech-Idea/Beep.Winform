using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Menu Fonts & Colors
        public Font MenuTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font MenuItemSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font MenuItemUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color MenuBackColor { get; set; } = Color.Black;
        public Color MenuForeColor { get; set; } = Color.White;
        public Color MenuBorderColor { get; set; } = Color.White;

        public Color MenuMainItemForeColor { get; set; } = Color.White;
        public Color MenuMainItemHoverForeColor { get; set; } = Color.Black;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.Yellow;
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.DarkBlue;

        public Color MenuItemForeColor { get; set; } = Color.White;
        public Color MenuItemHoverForeColor { get; set; } = Color.Black;
        public Color MenuItemHoverBackColor { get; set; } = Color.Yellow;
        public Color MenuItemSelectedForeColor { get; set; } = Color.Black;
        public Color MenuItemSelectedBackColor { get; set; } = Color.White;

        public Color MenuGradiantStartColor { get; set; } = Color.Black;
        public Color MenuGradiantEndColor { get; set; } = Color.DimGray;
        public Color MenuGradiantMiddleColor { get; set; } = Color.Gray;
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
