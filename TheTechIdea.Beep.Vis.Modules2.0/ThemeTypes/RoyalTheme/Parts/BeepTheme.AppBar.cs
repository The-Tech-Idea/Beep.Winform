using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color AppBarTextBoxForeColor { get; set; } = Color.White;
        public Color AppBarLabelForeColor { get; set; } = Color.White;
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent;
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(255, 77, 77);
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(108, 117, 125);
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
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
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(44, 48, 52);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}