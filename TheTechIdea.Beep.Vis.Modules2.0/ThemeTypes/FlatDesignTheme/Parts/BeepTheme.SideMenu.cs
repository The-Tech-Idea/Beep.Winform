using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16, FontStyle.Bold);
        public TypographyStyle SideMenuSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle SideMenuTextFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Regular);
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(30, 144, 255);  // DodgerBlue
        public Color SideMenuForeColor { get; set; } = Color.Black;
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.Black;
        public Color SideMenuBorderColor { get; set; } = Color.LightGray;
        public Color SideMenuTitleTextColor { get; set; } = Color.Black;
        public Color SideMenuTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Black,
            FontStyle = FontStyle.Regular
        };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.Gray;
        public Color SideMenuSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray,
            FontStyle = FontStyle.Regular
        };
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(220, 220, 220);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(230, 230, 230);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
