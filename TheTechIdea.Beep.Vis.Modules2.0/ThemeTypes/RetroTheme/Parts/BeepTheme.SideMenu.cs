using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Side Menu Fonts & Colors
        public TypographyStyle SideMenuTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 16, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle SideMenuSubTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(147, 161, 161), IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle SideMenuTextFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color SideMenuForeColor { get; set; } = Color.White;
        public Color SideMenuSelectedForeColor { get; set; } = Color.Black;
        public Color SideMenuHoverForeColor { get; set; } = Color.White;
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color SideMenuTitleTextColor { get; set; } = Color.White;
        public Color SideMenuTitleBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 16, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(147, 161, 161);
        public Color SideMenuSubTitleBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(147, 161, 161), IsUnderlined = false, IsStrikeout = false };
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(4, 48, 60);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}