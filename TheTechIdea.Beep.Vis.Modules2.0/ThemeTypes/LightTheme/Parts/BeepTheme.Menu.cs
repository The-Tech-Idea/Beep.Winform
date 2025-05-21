using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Menu Fonts & Colors
//<<<<<<< HEAD
        public Font MenuTitleFont { get; set; } = new Font("Segoe UI", 14F, FontStyle.Bold);
        public Font MenuItemSelectedFont { get; set; } = new Font("Segoe UI", 12F, FontStyle.Bold);
        public Font MenuItemUnSelectedFont { get; set; } = new Font("Segoe UI", 12F, FontStyle.Regular);
        public Color MenuBackColor { get; set; } = Color.White;
        public Color MenuForeColor { get; set; } = Color.Black;
        public Color MenuBorderColor { get; set; } = Color.LightGray;
        public Color MenuMainItemForeColor { get; set; } = Color.Black;
        public Color MenuMainItemHoverForeColor { get; set; } = Color.White;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.DodgerBlue;
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.RoyalBlue;
        public Color MenuItemForeColor { get; set; } = Color.Black;
        public Color MenuItemHoverForeColor { get; set; } = Color.White;
        public Color MenuItemHoverBackColor { get; set; } = Color.LightSkyBlue;
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color MenuGradiantStartColor { get; set; } = Color.White;
        public Color MenuGradiantEndColor { get; set; } = Color.LightGray;
        public Color MenuGradiantMiddleColor { get; set; } = Color.Gainsboro;
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
