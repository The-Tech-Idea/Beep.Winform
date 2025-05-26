using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(70, 70, 70),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuTextFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color SideMenuForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color SideMenuTitleTextColor { get; set; } = Color.FromArgb(25, 25, 112);
        public Color SideMenuTitleBackColor { get; set; } =Color.FromArgb(144, 238, 144);
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color SideMenuSubTitleBackColor { get; set; } =Color.FromArgb(144, 238, 144);
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(70, 70, 70),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(154, 211, 240);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}