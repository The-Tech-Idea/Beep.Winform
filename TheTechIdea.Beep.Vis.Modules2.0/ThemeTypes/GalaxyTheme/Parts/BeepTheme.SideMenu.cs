using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle  SideMenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  SideMenuSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);
        public TypographyStyle  SideMenuTextFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);

        public Color SideMenuBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Hover
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color SideMenuForeColor { get; set; } = Color.White;
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.White;
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Subtle border

        public Color SideMenuTitleTextColor { get; set; } = Color.White;
        public Color SideMenuTitleBackColor { get; set; } =Color.FromArgb(10, 10, 30);
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(0xC0, 0xC0, 0xFF); // Light violet
        public Color SideMenuSubTitleBackColor { get; set; } =Color.FromArgb(10, 10, 30);
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(0xC0, 0xC0, 0xFF),
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(0x1A, 0x1A, 0x2E); // PrimaryColor
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(0x16, 0x21, 0x3E); // SecondaryColor
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
