using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Menu Fonts & Colors
<<<<<<< HEAD
        public Font MenuTitleFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);
        public Font MenuItemSelectedFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font MenuItemUnSelectedFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Regular);
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
