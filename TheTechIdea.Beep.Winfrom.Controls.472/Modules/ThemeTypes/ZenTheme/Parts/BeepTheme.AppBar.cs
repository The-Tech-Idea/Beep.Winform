using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color AppBarLabelForeColor { get; set; } = Color.White;
        public Color AppBarLabelBackColor { get; set; } = Color.FromArgb(34, 34, 34); // Replaced Transparent
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } = Color.FromArgb(34, 34, 34); // Replaced Transparent
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color AppBarSubTitleBackColor { get; set; } = Color.FromArgb(34, 34, 34); // Replaced Transparent
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(244, 67, 54);
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(76, 175, 80);
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(48, 48, 48);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}