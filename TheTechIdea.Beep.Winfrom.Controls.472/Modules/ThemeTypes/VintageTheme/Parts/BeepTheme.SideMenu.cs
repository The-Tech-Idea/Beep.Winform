using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(90, 45, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuTextFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color SideMenuForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color SideMenuSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color SideMenuHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color SideMenuTitleTextColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color SideMenuTitleBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(90, 45, 0);
        public Color SideMenuSubTitleBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(90, 45, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(216, 194, 181);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}