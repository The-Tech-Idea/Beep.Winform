using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Menu Fonts & Colors
<<<<<<< HEAD
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
