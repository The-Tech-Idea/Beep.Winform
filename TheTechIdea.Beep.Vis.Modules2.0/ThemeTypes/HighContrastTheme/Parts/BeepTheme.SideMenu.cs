using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Side Menu Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  SideMenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  SideMenuSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  SideMenuTextFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Regular);
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
    }
}
