using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(34, 139, 34);               // ForestGreen
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(46, 139, 87);         // SeaGreen
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(60, 179, 113);       // MediumSeaGreen
        public Color AppBarTextBoxForeColor { get; set; } = Color.White;
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(152, 251, 152);        // PaleGreen
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent;
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(144, 238, 144);     // LightGreen
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent;
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(220, 20, 60);        // Crimson
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(50, 205, 50);          // LimeGreen
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(255, 215, 0);         // Gold

        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(144, 238, 144)
        };

        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.White
        };

        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(46, 139, 87);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(144, 238, 144);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
