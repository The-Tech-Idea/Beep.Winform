using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.LightGray;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(45, 45, 45);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color AppBarTextBoxForeColor { get; set; } = Color.WhiteSmoke;
        public Color AppBarLabelForeColor { get; set; } = Color.LightGray;
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent;
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarSubTitleForeColor { get; set; } = Color.Gray;
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(200, 50, 50);
        public Color AppBarMaxButtonColor { get; set; } = Color.LightGray;
        public Color AppBarMinButtonColor { get; set; } = Color.LightGray;

        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White,
            FontStyle = FontStyle.Regular
        };

        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray,
            FontStyle = FontStyle.Italic
        };

        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.WhiteSmoke,
            FontStyle = FontStyle.Regular
        };

        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(20, 20, 20);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
