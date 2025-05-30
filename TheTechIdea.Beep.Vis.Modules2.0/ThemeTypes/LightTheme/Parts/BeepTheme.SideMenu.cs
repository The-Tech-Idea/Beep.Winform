using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Side Menu Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  SideMenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16, FontStyle.Bold);
        public TypographyStyle  SideMenuSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Regular);
        public TypographyStyle  SideMenuTextFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color SideMenuBackColor { get; set; } = Color.WhiteSmoke;
        public Color SideMenuHoverBackColor { get; set; } = Color.LightGray;
        public Color SideMenuSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color SideMenuForeColor { get; set; } = Color.Black;
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.DimGray;
        public Color SideMenuBorderColor { get; set; } = Color.LightGray;
        public Color SideMenuTitleTextColor { get; set; } = Color.Black;
        public Color SideMenuTitleBackColor { get; set; } =Color.White;
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Black
        };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.DimGray;
        public Color SideMenuSubTitleBackColor { get; set; } =Color.White;
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.DimGray
        };
        public Color SideMenuGradiantStartColor { get; set; } = Color.WhiteSmoke;
        public Color SideMenuGradiantEndColor { get; set; } = Color.Gainsboro;
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.LightGray;
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
