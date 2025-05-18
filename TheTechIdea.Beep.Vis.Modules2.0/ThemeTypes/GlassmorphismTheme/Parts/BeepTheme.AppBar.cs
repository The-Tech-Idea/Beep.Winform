using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(220, 230, 240); // Light glassy blue
        public Color AppBarForeColor { get; set; } = Color.Black;
        public Color AppBarButtonForeColor { get; set; } = Color.Black;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(200, 210, 220);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(245, 245, 250);
        public Color AppBarTextBoxForeColor { get; set; } = Color.Black;
        public Color AppBarLabelForeColor { get; set; } = Color.Black;
        public Color AppBarLabelBackColor { get; set; } = Color.FromArgb(235, 240, 245);
        public Color AppBarTitleForeColor { get; set; } = Color.Black;
        public Color AppBarTitleBackColor { get; set; } = Color.FromArgb(225, 235, 245);
        public Color AppBarSubTitleForeColor { get; set; } = Color.Gray;
        public Color AppBarSubTitleBackColor { get; set; } = Color.FromArgb(230, 235, 240);
        public Color AppBarCloseButtonColor { get; set; } = Color.Red;
        public Color AppBarMaxButtonColor { get; set; } = Color.Green;
        public Color AppBarMinButtonColor { get; set; } = Color.Goldenrod;

        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.SemiBold,
            FontStyle = FontStyle.Regular,
            LineHeight = 1.2f,
            LetterSpacing = 0f,
            TextColor = Color.Black
        };

        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Italic,
            LineHeight = 1.2f,
            LetterSpacing = 0f,
            TextColor = Color.DimGray
        };

        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            LineHeight = 1.1f,
            LetterSpacing = 0f,
            TextColor = Color.Black
        };

        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(210, 220, 235);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(180, 200, 220);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(200, 215, 230);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
