using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; }
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; }
        public Color AppBarButtonBackColor { get; set; }
        public Color AppBarTextBoxBackColor { get; set; }
        public Color AppBarTextBoxForeColor { get; set; }
        public Color AppBarLabelForeColor { get; set; }
        public Color AppBarLabelBackColor { get; set; }
        public Color AppBarTitleForeColor { get; set; }
        public Color AppBarTitleBackColor { get; set; }
        public Color AppBarSubTitleForeColor { get; set; }
        public Color AppBarSubTitleBackColor { get; set; }
        public Color AppBarCloseButtonColor { get; set; }
        public Color AppBarMaxButtonColor { get; set; }
        public Color AppBarMinButtonColor { get; set; }
        public TypographyStyle AppBarTitleStyle { get; set; }
        public TypographyStyle AppBarSubTitleStyle { get; set; }
        public TypographyStyle AppBarTextStyle { get; set; }
        public Color AppBarGradiantStartColor { get; set; }
        public Color AppBarGradiantEndColor { get; set; }
        public Color AppBarGradiantMiddleColor { get; set; }
        public LinearGradientMode AppBarGradiantDirection { get; set; }
    }
}
