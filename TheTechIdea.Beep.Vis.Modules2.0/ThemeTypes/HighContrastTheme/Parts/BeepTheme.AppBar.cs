using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.Black;
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.DarkSlateGray;
        public Color AppBarTextBoxBackColor { get; set; } = Color.White;
        public Color AppBarTextBoxForeColor { get; set; } = Color.Black;
        public Color AppBarLabelForeColor { get; set; } = Color.White;
        public Color AppBarLabelBackColor { get; set; } = Color.Black;
        public Color AppBarTitleForeColor { get; set; } = Color.Yellow;
        public Color AppBarTitleBackColor { get; set; } = Color.Black;
        public Color AppBarSubTitleForeColor { get; set; } = Color.Orange;
        public Color AppBarSubTitleBackColor { get; set; } = Color.Black;
        public Color AppBarCloseButtonColor { get; set; } = Color.Red;
        public Color AppBarMaxButtonColor { get; set; } = Color.Green;
        public Color AppBarMinButtonColor { get; set; } = Color.Goldenrod;
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Yellow
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Orange
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.Black;
        public Color AppBarGradiantEndColor { get; set; } = Color.DarkSlateGray;
        public Color AppBarGradiantMiddleColor { get; set; } = Color.DimGray;
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
