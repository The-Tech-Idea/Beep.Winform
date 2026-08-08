using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; }
        public TypographyStyle SideMenuSubTitleFont { get; set; }
        public TypographyStyle SideMenuTextFont { get; set; }
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color SideMenuForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color SideMenuTitleTextColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color SideMenuTitleBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public TypographyStyle SideMenuTitleStyle { get; set; }
        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color SideMenuSubTitleBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public TypographyStyle SideMenuSubTitleStyle { get; set; }
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(210, 180, 140);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(238, 232, 205);
        public LinearGradientMode SideMenuGradiantDirection { get; set; }
    }
}
