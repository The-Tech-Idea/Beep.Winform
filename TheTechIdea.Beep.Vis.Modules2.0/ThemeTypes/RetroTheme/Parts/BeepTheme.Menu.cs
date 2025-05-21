using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MenuItemSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color MenuBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color MenuForeColor { get; set; } = Color.White;
        public Color MenuBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color MenuMainItemHoverForeColor { get; set; } = Color.White;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color MenuItemHoverForeColor { get; set; } = Color.White;
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(32, 32, 32);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(48, 48, 48);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}