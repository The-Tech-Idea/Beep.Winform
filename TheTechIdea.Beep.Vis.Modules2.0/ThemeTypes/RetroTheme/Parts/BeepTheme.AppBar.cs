using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(32, 32, 32);
        public Color AppBarTextBoxForeColor { get; set; } = Color.White;
        public Color AppBarLabelForeColor { get; set; } = Color.White;
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent;
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(255, 64, 64);
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(64, 255, 64);
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(64, 64, 255);
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
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
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(32, 32, 32);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(48, 48, 48);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}