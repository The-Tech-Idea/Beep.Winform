using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle SideMenuSubTitleFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public TypographyStyle SideMenuTextFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color SideMenuForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.White;
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color SideMenuTitleTextColor { get; set; } = Color.White;
        public Color SideMenuTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color SideMenuSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(0, 105, 148);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}