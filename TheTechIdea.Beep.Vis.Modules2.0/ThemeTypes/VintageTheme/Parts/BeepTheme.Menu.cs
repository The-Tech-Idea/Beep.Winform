using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MenuItemSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color MenuBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color MenuForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color MenuBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color MenuMainItemHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color MenuItemHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color MenuItemSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(216, 194, 181);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}