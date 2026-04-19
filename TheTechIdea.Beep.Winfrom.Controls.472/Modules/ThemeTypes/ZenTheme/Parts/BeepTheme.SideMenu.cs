using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuTextFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color SideMenuForeColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.White;
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color SideMenuTitleTextColor { get; set; } = Color.White;
        public Color SideMenuTitleBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color SideMenuSubTitleBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(48, 48, 48);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}