using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(255, 255, 204); // light yellow
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.Black;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(255, 204, 0); // bright yellow
        public Color AppBarTextBoxBackColor { get; set; } = Color.White;
        public Color AppBarTextBoxForeColor { get; set; } = Color.Black;
        public Color AppBarLabelForeColor { get; set; } = Color.Black;
        public Color AppBarLabelBackColor { get; set; } =Color.FromArgb(255, 255, 204);
        public Color AppBarTitleForeColor { get; set; } = Color.Black;
        public Color AppBarTitleBackColor { get; set; } =Color.FromArgb(255, 255, 204);
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(102, 102, 102);
        public Color AppBarSubTitleBackColor { get; set; } =Color.FromArgb(255, 255, 204);
        public Color AppBarCloseButtonColor { get; set; } = Color.Red;
        public Color AppBarMaxButtonColor { get; set; } = Color.Green;
        public Color AppBarMinButtonColor { get; set; } = Color.Orange;
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Black
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Black
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(255, 255, 204);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(255, 204, 0);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(255, 229, 102);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}
