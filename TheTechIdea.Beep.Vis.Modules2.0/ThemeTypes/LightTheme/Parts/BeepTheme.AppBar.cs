using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // AppBar colors and styles with default light theme colors
        public Color AppBarBackColor { get; set; } = Color.White;
        public Color AppBarForeColor { get; set; } = Color.Black;
        public Color AppBarButtonForeColor { get; set; } = Color.Black;
        public Color AppBarButtonBackColor { get; set; } = Color.LightGray;
        public Color AppBarTextBoxBackColor { get; set; } = Color.White;
        public Color AppBarTextBoxForeColor { get; set; } = Color.Black;
        public Color AppBarLabelForeColor { get; set; } = Color.Black;
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent;
        public Color AppBarTitleForeColor { get; set; } = Color.Black;
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarSubTitleForeColor { get; set; } = Color.Gray;
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarCloseButtonColor { get; set; } = Color.Red;
        public Color AppBarMaxButtonColor { get; set; } = Color.Green;
        public Color AppBarMinButtonColor { get; set; } = Color.YellowGreen;

        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Black
        };

        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray
        };

        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Black
        };

        public Color AppBarGradiantStartColor { get; set; } = Color.WhiteSmoke;
        public Color AppBarGradiantEndColor { get; set; } = Color.LightGray;
        public Color AppBarGradiantMiddleColor { get; set; } = Color.Gainsboro;
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}
