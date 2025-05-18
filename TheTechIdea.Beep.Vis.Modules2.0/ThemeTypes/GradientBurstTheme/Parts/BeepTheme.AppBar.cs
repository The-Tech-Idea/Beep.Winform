using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(58, 123, 213);
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(45, 100, 200);
        public Color AppBarTextBoxBackColor { get; set; } = Color.White;
        public Color AppBarTextBoxForeColor { get; set; } = Color.Black;
        public Color AppBarLabelForeColor { get; set; } = Color.White;
        public Color AppBarLabelBackColor { get; set; } = Color.FromArgb(45, 100, 200);
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } = Color.FromArgb(30, 90, 180);
        public Color AppBarSubTitleForeColor { get; set; } = Color.WhiteSmoke;
        public Color AppBarSubTitleBackColor { get; set; } = Color.FromArgb(40, 100, 190);
        public Color AppBarCloseButtonColor { get; set; } = Color.Red;
        public Color AppBarMaxButtonColor { get; set; } = Color.YellowGreen;
        public Color AppBarMinButtonColor { get; set; } = Color.Goldenrod;

        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0
        };

        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 13,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.WhiteSmoke,
            LineHeight = 1.1f,
            LetterSpacing = 0
        };

        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.0f,
            LetterSpacing = 0
        };

        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(40, 100, 190);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(20, 60, 120);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(30, 80, 160);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
