using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // AppBar colors and styles - warm, earthy desert palette
        public Color AppBarBackColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color AppBarForeColor { get; set; } = Color.FromArgb(60, 30, 10); // Dark Brown
        public Color AppBarButtonForeColor { get; set; } = Color.FromArgb(60, 30, 10);
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(244, 164, 96); // Sandy Brown
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(255, 245, 230); // Light Sand
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(60, 30, 10);
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(60, 30, 10);
        public Color AppBarLabelBackColor { get; set; } =Color.FromArgb(210, 180, 140);
        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(51, 34, 17); // Deep Brown
        public Color AppBarTitleBackColor { get; set; } =Color.FromArgb(210, 180, 140);
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(120, 90, 60); // Muted Brown
        public Color AppBarSubTitleBackColor { get; set; } =Color.FromArgb(210, 180, 140);
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(178, 34, 34); // Firebrick red
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(210, 105, 30); // Chocolate
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(244, 164, 96); // Sandy Brown

        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = Color.FromArgb(51, 34, 17),
            LineHeight = 1.2f
        };

        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = Color.FromArgb(120, 90, 60),
            LineHeight = 1.1f
        };

        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = Color.FromArgb(60, 30, 10),
            LineHeight = 1.0f
        };

        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(210, 180, 140);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(244, 164, 96);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(222, 184, 135);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}
