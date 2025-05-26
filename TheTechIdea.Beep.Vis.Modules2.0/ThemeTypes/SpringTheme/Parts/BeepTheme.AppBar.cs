using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color AppBarLabelBackColor { get; set; } =Color.FromArgb(144, 238, 144);
        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(25, 25, 112);
        public Color AppBarTitleBackColor { get; set; } =Color.FromArgb(144, 238, 144);
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color AppBarSubTitleBackColor { get; set; } =Color.FromArgb(144, 238, 144);
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(255, 99, 71);
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(255, 215, 0);
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(70, 70, 70),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(135, 206, 250);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(154, 211, 240);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}