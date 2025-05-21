using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color AppBarLabelForeColor { get; set; } = Color.White;
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent;
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(255, 150, 150);
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(150, 255, 150);
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(150, 150, 255);
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(240, 240, 240) };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.White };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(247, 221, 229);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(242, 201, 215);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}