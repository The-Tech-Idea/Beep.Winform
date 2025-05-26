using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16, FontStyle.Bold);
        public TypographyStyle SideMenuSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Italic);
        public TypographyStyle SideMenuTextFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color SideMenuBackColor { get; set; } = Color.FromArgb(25, 25, 25);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(70, 70, 70);

        public Color SideMenuForeColor { get; set; } = Color.LightGray;
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.WhiteSmoke;

        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(80, 80, 80);

        public Color SideMenuTitleTextColor { get; set; } = Color.White;
        public Color SideMenuTitleBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public Color SideMenuSubTitleTextColor { get; set; } = Color.Gray;
        public Color SideMenuSubTitleBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontStyle = FontStyle.Italic,
            TextColor = Color.Gray
        };

        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(20, 20, 20);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(30, 30, 30);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
