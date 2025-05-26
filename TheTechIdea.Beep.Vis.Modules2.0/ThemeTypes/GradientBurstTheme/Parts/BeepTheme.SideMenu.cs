using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Side Menu Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  SideMenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16, FontStyle.Bold);
        public TypographyStyle  SideMenuSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Italic);
        public TypographyStyle  SideMenuTextFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color SideMenuBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 212);

        public Color SideMenuForeColor { get; set; } = Color.WhiteSmoke;
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.FromArgb(200, 230, 255);

        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color SideMenuTitleTextColor { get; set; } = Color.White;
        public Color SideMenuTitleBackColor { get; set; } = Color.FromArgb(45, 45, 48);
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White
        };

        public Color SideMenuSubTitleTextColor { get; set; } = Color.LightGray;
        public Color SideMenuSubTitleBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.LightGray
        };

        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(0, 102, 204);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
