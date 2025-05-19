using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 16, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle MenuItemSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color MenuBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color MenuForeColor { get; set; } = Color.White;
        public Color MenuBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color MenuMainItemForeColor { get; set; } = Color.White;
        public Color MenuMainItemHoverForeColor { get; set; } = Color.White;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.Black;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color MenuItemForeColor { get; set; } = Color.White;
        public Color MenuItemHoverForeColor { get; set; } = Color.White;
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color MenuItemSelectedForeColor { get; set; } = Color.Black;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(4, 48, 60);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}