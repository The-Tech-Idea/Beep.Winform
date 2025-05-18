using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(10, 10, 30); // Dark navy-blue
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.FromArgb(180, 180, 255);
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(25, 25, 60);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(35, 35, 70);
        public Color AppBarTextBoxForeColor { get; set; } = Color.White;
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(180, 180, 255);
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent;
        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(200, 200, 255);
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(160, 160, 200);
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(255, 85, 85); // subtle red
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(100, 180, 255); // bright blue
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(100, 180, 255);
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(200, 200, 255)
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(160, 160, 200)
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.White
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(10, 10, 30);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(40, 40, 90);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(25, 25, 60);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
