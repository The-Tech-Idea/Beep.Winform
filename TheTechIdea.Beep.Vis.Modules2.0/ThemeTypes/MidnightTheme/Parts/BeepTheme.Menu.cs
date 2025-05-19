using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Menu Fonts & Colors
<<<<<<< HEAD
        public Font MenuTitleFont { get; set; } = new Font("Segoe UI", 16f, FontStyle.Bold);
        public Font MenuItemSelectedFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Font MenuItemUnSelectedFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Regular);

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
=======
        public TypographyStyle MenuTitleFont { get; set; }
        public TypographyStyle MenuItemSelectedFont { get; set; }
        public TypographyStyle MenuItemUnSelectedFont { get; set; }
        public Color MenuBackColor { get; set; }
        public Color MenuForeColor { get; set; }
        public Color MenuBorderColor { get; set; }
        public Color MenuMainItemForeColor { get; set; }
        public Color MenuMainItemHoverForeColor { get; set; }
        public Color MenuMainItemHoverBackColor { get; set; }
        public Color MenuMainItemSelectedForeColor { get; set; }
        public Color MenuMainItemSelectedBackColor { get; set; }
        public Color MenuItemForeColor { get; set; }
        public Color MenuItemHoverForeColor { get; set; }
        public Color MenuItemHoverBackColor { get; set; }
        public Color MenuItemSelectedForeColor { get; set; }
        public Color MenuItemSelectedBackColor { get; set; }
        public Color MenuGradiantStartColor { get; set; }
        public Color MenuGradiantEndColor { get; set; }
        public Color MenuGradiantMiddleColor { get; set; }
        public LinearGradientMode MenuGradiantDirection { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
