using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Side Menu Fonts & Colors
<<<<<<< HEAD
        public Font SideMenuTitleFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);
        public Font SideMenuSubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Font SideMenuTextFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);

        public Color SideMenuBackColor { get; set; } = Color.FromArgb(20, 20, 30);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(50, 50, 70);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(80, 80, 110);

        public Color SideMenuForeColor { get; set; } = Color.LightGray;
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.Gainsboro;

        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(70, 70, 90);
        public Color SideMenuTitleTextColor { get; set; } = Color.WhiteSmoke;
        public Color SideMenuTitleBackColor { get; set; } = Color.Transparent;

        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.WhiteSmoke
        };

        public Color SideMenuSubTitleTextColor { get; set; } = Color.LightGray;
        public Color SideMenuSubTitleBackColor { get; set; } = Color.Transparent;

        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };

        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(20, 20, 30);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(40, 40, 60);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(30, 30, 45);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
=======
        public TypographyStyle SideMenuTitleFont { get; set; }
        public TypographyStyle SideMenuSubTitleFont { get; set; }
        public TypographyStyle SideMenuTextFont { get; set; }
        public Color SideMenuBackColor { get; set; }
        public Color SideMenuHoverBackColor { get; set; }
        public Color SideMenuSelectedBackColor { get; set; }
        public Color SideMenuForeColor { get; set; }
        public Color SideMenuSelectedForeColor { get; set; }
        public Color SideMenuHoverForeColor { get; set; }
        public Color SideMenuBorderColor { get; set; }
        public Color SideMenuTitleTextColor { get; set; }
        public Color SideMenuTitleBackColor { get; set; }
        public TypographyStyle SideMenuTitleStyle { get; set; }
        public Color SideMenuSubTitleTextColor { get; set; }
        public Color SideMenuSubTitleBackColor { get; set; }
        public TypographyStyle SideMenuSubTitleStyle { get; set; }
        public Color SideMenuGradiantStartColor { get; set; }
        public Color SideMenuGradiantEndColor { get; set; }
        public Color SideMenuGradiantMiddleColor { get; set; }
        public LinearGradientMode SideMenuGradiantDirection { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
