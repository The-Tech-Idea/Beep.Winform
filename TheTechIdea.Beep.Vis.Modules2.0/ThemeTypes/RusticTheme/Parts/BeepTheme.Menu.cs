using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(70, 50, 30),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MenuItemSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 220),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(90, 70, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color MenuBackColor { get; set; } = Color.FromArgb(240, 230, 210);
        public Color MenuForeColor { get; set; } = Color.FromArgb(90, 70, 50);
        public Color MenuBorderColor { get; set; } = Color.FromArgb(150, 130, 110);
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(90, 70, 50);
        public Color MenuMainItemHoverForeColor { get; set; } = Color.FromArgb(255, 245, 220);
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(200, 180, 160);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 220);
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(180, 160, 140);
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(90, 70, 50);
        public Color MenuItemHoverForeColor { get; set; } = Color.FromArgb(255, 245, 220);
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(200, 180, 160);
        public Color MenuItemSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 220);
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(180, 160, 140);
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(245, 235, 215);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(220, 210, 190);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(230, 220, 200);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}