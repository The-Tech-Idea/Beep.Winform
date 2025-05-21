using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle SideMenuSubTitleFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public TypographyStyle SideMenuTextFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color SideMenuForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color SideMenuTitleTextColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color SideMenuTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color SideMenuSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(247, 221, 229);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(242, 201, 215);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}