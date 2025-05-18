using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Flat Blue
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(25, 118, 210); // Darker Blue
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(227, 242, 253); // Light Blue background
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark text
        public Color AppBarLabelForeColor { get; set; } = Color.White;
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent;
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(200, 200, 200); // Slightly lighter grey
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(244, 67, 54); // Flat red
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(76, 175, 80); // Flat green
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(255, 235, 59); // Flat yellow
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(200, 200, 200)
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.White
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.Transparent;
        public Color AppBarGradiantEndColor { get; set; } = Color.Transparent;
        public Color AppBarGradiantMiddleColor { get; set; } = Color.Transparent;
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}
