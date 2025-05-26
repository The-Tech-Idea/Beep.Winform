using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Side Menu Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  SideMenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle  SideMenuSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Italic);
        public TypographyStyle  SideMenuTextFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);

        public Color SideMenuBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color SideMenuHoverBackColor { get; set; } = Color.LightBlue;
        public Color SideMenuSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color SideMenuForeColor { get; set; } = Color.Black;
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.Black;
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(200, 210, 220);

        public Color SideMenuTitleTextColor { get; set; } = Color.Black;
        public Color SideMenuTitleBackColor { get; set; } = Color.WhiteSmoke;
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Black,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color SideMenuSubTitleTextColor { get; set; } = Color.DimGray;
        public Color SideMenuSubTitleBackColor { get; set; } = Color.WhiteSmoke;
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Italic,
            TextColor = Color.DimGray,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(215, 230, 245);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
