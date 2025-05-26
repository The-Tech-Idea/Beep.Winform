using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color AppBarLabelBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color AppBarTitleBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(245, 222, 179);
        public Color AppBarSubTitleBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(188, 143, 143);
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 20,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(245, 222, 179),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(150, 75, 32);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}