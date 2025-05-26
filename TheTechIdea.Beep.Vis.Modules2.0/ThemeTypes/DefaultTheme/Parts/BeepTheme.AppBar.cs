using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(33, 150, 243);            // Modern blue
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(25, 118, 210);     // Slightly darker blue for buttons
        public Color AppBarTextBoxBackColor { get; set; } = Color.White;
        public Color AppBarTextBoxForeColor { get; set; } = Color.Black;
        public Color AppBarLabelForeColor { get; set; } = Color.White;
        public Color AppBarLabelBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(187, 222, 251);   // Light blue subtitle
        public Color AppBarSubTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(244, 67, 54);      // Red close button
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(76, 175, 80);        // Green max button
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(255, 235, 59);       // Yellow minimize button
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.White,
            LineHeight = 1.2f
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(187, 222, 251),
            LineHeight = 1.1f
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.White,
            LineHeight = 1.15f
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(33, 150, 243);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(30, 136, 229);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
