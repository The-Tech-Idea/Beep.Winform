using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle SideMenuSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);
        public TypographyStyle SideMenuTextFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color SideMenuBackColor { get; set; } = Color.FromArgb(34, 49, 34); // Dark green
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(56, 142, 60); // Medium green hover
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(102, 187, 106); // Bright green selected

        public Color SideMenuForeColor { get; set; } = Color.WhiteSmoke;
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.LightGreen;

        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(46, 71, 46); // Dark green border

        public Color SideMenuTitleTextColor { get; set; } = Color.White;
        public Color SideMenuTitleBackColor { get; set; } = Color.FromArgb(21, 33, 21); // Very dark green background

        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public Color SideMenuSubTitleTextColor { get; set; } = Color.LightGray;
        public Color SideMenuSubTitleBackColor { get; set; } = Color.FromArgb(28, 41, 28); // Dark greenish

        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontStyle = FontStyle.Italic,
            TextColor = Color.LightGray
        };

        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(21, 33, 21);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(56, 142, 60);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(34, 49, 34);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
