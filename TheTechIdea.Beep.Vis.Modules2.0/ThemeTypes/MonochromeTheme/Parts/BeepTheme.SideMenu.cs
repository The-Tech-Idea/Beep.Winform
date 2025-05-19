using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16f, FontStyle.Bold);
        public TypographyStyle SideMenuSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);
        public TypographyStyle SideMenuTextFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11f, FontStyle.Regular);

        public Color SideMenuBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(80, 80, 80);

        public Color SideMenuForeColor { get; set; } = Color.LightGray;
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.Gainsboro;

        public Color SideMenuBorderColor { get; set; } = Color.DimGray;

        public Color SideMenuTitleTextColor { get; set; } = Color.WhiteSmoke;
        public Color SideMenuTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.WhiteSmoke,
            IsUnderlined = false,
            IsStrikeout = false,
            FontStyle = FontStyle.Regular
        };

        public Color SideMenuSubTitleTextColor { get; set; } = Color.Gray;
        public Color SideMenuSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray,
            IsUnderlined = false,
            IsStrikeout = false,
            FontStyle = FontStyle.Italic
        };

        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(20, 20, 20);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(30, 30, 30);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
