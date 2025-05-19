using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Side Menu Fonts & Colors
<<<<<<< HEAD
        public Font SideMenuTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font SideMenuSubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font SideMenuTextFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);
        public Color SideMenuBackColor { get; set; } = Color.Black;
        public Color SideMenuHoverBackColor { get; set; } = Color.DarkSlateGray;
        public Color SideMenuSelectedBackColor { get; set; } = Color.Yellow;
        public Color SideMenuForeColor { get; set; } = Color.White;
        public Color SideMenuSelectedForeColor { get; set; } = Color.Black;
        public Color SideMenuHoverForeColor { get; set; } = Color.Yellow;
        public Color SideMenuBorderColor { get; set; } = Color.White;
        public Color SideMenuTitleTextColor { get; set; } = Color.White;
        public Color SideMenuTitleBackColor { get; set; } = Color.Black;
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Bold,
            TextColor = Color.White
        };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.Gray;
        public Color SideMenuSubTitleBackColor { get; set; } = Color.Black;
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.Gray
        };
        public Color SideMenuGradiantStartColor { get; set; } = Color.Black;
        public Color SideMenuGradiantEndColor { get; set; } = Color.DimGray;
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.Gray;
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
