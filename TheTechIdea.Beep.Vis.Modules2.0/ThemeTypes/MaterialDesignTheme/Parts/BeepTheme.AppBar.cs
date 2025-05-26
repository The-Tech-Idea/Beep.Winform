using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Material Blue 500
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(227, 242, 253); // Light Blue 50
        public Color AppBarTextBoxForeColor { get; set; } = Color.Black;
        public Color AppBarLabelForeColor { get; set; } = Color.White;
        public Color AppBarLabelBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(187, 222, 251); // Light Blue 200
        public Color AppBarSubTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color AppBarCloseButtonColor { get; set; } = Color.White;
        public Color AppBarMaxButtonColor { get; set; } = Color.White;
        public Color AppBarMinButtonColor { get; set; } = Color.White;
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 20f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.White
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(187, 222, 251) // Light Blue 200
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.White
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(33, 150, 243);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(30, 136, 229);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
