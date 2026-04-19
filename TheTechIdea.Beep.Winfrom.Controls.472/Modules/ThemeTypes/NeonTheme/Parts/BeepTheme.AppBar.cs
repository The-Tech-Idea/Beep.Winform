using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(30, 30, 50);
        public Color AppBarForeColor { get; set; } = Color.FromArgb(236, 240, 241);
        public Color AppBarButtonForeColor { get; set; } = Color.FromArgb(26, 188, 156);
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(40, 40, 60);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(40, 40, 70);
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(46, 204, 113);
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(155, 89, 182);
        public Color AppBarLabelBackColor { get; set; } = Color.FromArgb(40, 40, 60);
        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(26, 188, 156);
        public Color AppBarTitleBackColor { get; set; } = Color.FromArgb(40, 40, 60);
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(46, 204, 113);
        public Color AppBarSubTitleBackColor { get; set; } = Color.FromArgb(40, 40, 60);
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(231, 76, 60);
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(46, 204, 113);
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(241, 196, 15);

        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156),
            LineHeight = 1.3f,
            LetterSpacing = 0.5f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(46, 204, 113),
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(236, 240, 241),
            LineHeight = 1.1f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(26, 188, 156);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(46, 204, 113);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(155, 89, 182);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}