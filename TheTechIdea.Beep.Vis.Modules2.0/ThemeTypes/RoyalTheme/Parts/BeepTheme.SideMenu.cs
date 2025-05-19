using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuTextFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color SideMenuForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.White;
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color SideMenuTitleTextColor { get; set; } = Color.White;
        public Color SideMenuTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color SideMenuSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(44, 48, 52);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}