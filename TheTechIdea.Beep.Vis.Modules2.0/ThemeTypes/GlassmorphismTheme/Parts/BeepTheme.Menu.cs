using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Menu Fonts & Colors
<<<<<<< HEAD
        public Font MenuTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font MenuItemSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font MenuItemUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color MenuBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color MenuForeColor { get; set; } = Color.Black;
        public Color MenuBorderColor { get; set; } = Color.FromArgb(200, 210, 220);

        public Color MenuMainItemForeColor { get; set; } = Color.Black;
        public Color MenuMainItemHoverForeColor { get; set; } = Color.White;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.SteelBlue;
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.DodgerBlue;

        public Color MenuItemForeColor { get; set; } = Color.Black;
        public Color MenuItemHoverForeColor { get; set; } = Color.White;
        public Color MenuItemHoverBackColor { get; set; } = Color.LightSkyBlue;
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.DeepSkyBlue;

        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(215, 230, 245);
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
