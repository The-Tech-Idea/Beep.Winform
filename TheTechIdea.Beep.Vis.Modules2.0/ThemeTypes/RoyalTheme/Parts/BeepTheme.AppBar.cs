using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color AppBarForeColor { get; set; } = Color.White;
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(70, 70, 130); // Muted indigo
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(25, 25, 112);
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(230, 230, 250); // Lavender white
        public Color AppBarLabelBackColor { get; set; } =Color.FromArgb(70, 70, 130);
        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color AppBarTitleBackColor { get; set; } =Color.FromArgb(70, 70, 130);
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color AppBarSubTitleBackColor { get; set; } =Color.FromArgb(70, 70, 130);
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(178, 34, 34); // Crimson
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(0, 128, 0); // Emerald
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(70, 70, 130); // Indigo
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 18,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(200, 200, 220),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(230, 230, 250),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(25, 25, 112);
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(45, 45, 128);
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}