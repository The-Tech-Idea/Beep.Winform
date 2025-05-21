using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(20, 24, 30);
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.WhiteSmoke;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(40, 44, 52);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(30, 34, 40);
        public Color AppBarTextBoxForeColor { get; set; } = Color.LightGray;
        public Color AppBarLabelForeColor { get; set; } = Color.LightGray;
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent;
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarSubTitleForeColor { get; set; } = Color.Gray;
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarCloseButtonColor { get; set; } = Color.Red;
        public Color AppBarMaxButtonColor { get; set; } = Color.LightGray;
        public Color AppBarMinButtonColor { get; set; } = Color.LightGray;
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.White
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(20, 24, 30);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(40, 44, 52);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(30, 34, 40);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
