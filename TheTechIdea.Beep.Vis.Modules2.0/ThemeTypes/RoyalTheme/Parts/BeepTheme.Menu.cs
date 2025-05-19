using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MenuItemSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color MenuBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color MenuForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color MenuBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color MenuMainItemHoverForeColor { get; set; } = Color.White;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color MenuItemHoverForeColor { get; set; } = Color.White;
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(44, 48, 52);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}