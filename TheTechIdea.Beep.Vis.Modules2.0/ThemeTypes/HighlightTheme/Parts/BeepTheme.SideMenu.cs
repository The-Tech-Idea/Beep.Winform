using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Side Menu Fonts & Colors
//<<<<<<< HEAD
        public Font SideMenuTitleFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);
        public Font SideMenuSubTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Regular);
        public Font SideMenuTextFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color SideMenuBackColor { get; set; } = Color.White;
        public Color SideMenuHoverBackColor { get; set; } = Color.LightBlue;
        public Color SideMenuSelectedBackColor { get; set; } = Color.Blue;
        public Color SideMenuForeColor { get; set; } = Color.Black;
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.DarkBlue;
        public Color SideMenuBorderColor { get; set; } = Color.Gray;
        public Color SideMenuTitleTextColor { get; set; } = Color.Black;
        public Color SideMenuTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle();
        public Color SideMenuSubTitleTextColor { get; set; } = Color.DarkGray;
        public Color SideMenuSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle();
        public Color SideMenuGradiantStartColor { get; set; } = Color.White;
        public Color SideMenuGradiantEndColor { get; set; } = Color.LightGray;
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.Gray;
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
