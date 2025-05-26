using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public Color AppBarLabelForeColor { get; set; } = Color.White;
        public Color AppBarLabelBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public Color AppBarTitleForeColor { get; set; } = Color.White;
        public Color AppBarTitleBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color AppBarSubTitleBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(178, 34, 34); // Firebrick
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(107, 142, 35); // OliveDrab
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(107, 142, 35); // OliveDrab
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 20,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(245, 245, 220), // Beige
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(149, 75, 32); // Midpoint Brown
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}