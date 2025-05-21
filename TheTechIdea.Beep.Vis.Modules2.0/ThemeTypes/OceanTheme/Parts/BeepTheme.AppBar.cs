using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(255, 255, 255, 200);
        public Color AppBarTextBoxForeColor { get; set; } = Color.Black;
        public Color AppBarLabelForeColor { get; set; } = Color.White;
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent;
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(255, 80, 80);
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(80, 200, 80);
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(80, 80, 255);
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.White };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(0, 105, 148);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}