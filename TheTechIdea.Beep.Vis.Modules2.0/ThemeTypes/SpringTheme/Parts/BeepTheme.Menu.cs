using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MenuItemSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
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
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color MenuBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color MenuForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color MenuBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color MenuMainItemHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color MenuItemHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(154, 211, 240);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}