using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(45, 85, 120);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color AppBarLabelForeColor { get; set; } = Color.White;
        public Color AppBarLabelBackColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color AppBarSubTitleBackColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(255, 99, 99);
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(100, 149, 237);
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
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
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(45, 85, 120);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(36, 73, 106);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}