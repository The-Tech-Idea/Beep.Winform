using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // AppBar colors and styles with default monochrome values
        public Color AppBarBackColor { get; set; } = Color.Black;
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.WhiteSmoke;
        public Color AppBarButtonBackColor { get; set; } = Color.DimGray;
        public Color AppBarTextBoxBackColor { get; set; } = Color.DarkGray;
        public Color AppBarTextBoxForeColor { get; set; } = Color.WhiteSmoke;
        public Color AppBarLabelForeColor { get; set; } = Color.Gainsboro;
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent;
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarSubTitleForeColor { get; set; } = Color.LightGray;
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarCloseButtonColor { get; set; } = Color.LightCoral;
        public Color AppBarMaxButtonColor { get; set; } = Color.LightGray;
        public Color AppBarMinButtonColor { get; set; } = Color.LightGray;
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.WhiteSmoke
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.Black;
        public Color AppBarGradiantEndColor { get; set; } = Color.DimGray;
        public Color AppBarGradiantMiddleColor { get; set; } = Color.Gray;
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}
