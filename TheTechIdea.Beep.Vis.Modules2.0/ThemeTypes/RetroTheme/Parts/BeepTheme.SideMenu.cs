using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuTextFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color SideMenuForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.White;
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color SideMenuTitleTextColor { get; set; } = Color.White;
        public Color SideMenuTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color SideMenuSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(32, 32, 32);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(48, 48, 48);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}