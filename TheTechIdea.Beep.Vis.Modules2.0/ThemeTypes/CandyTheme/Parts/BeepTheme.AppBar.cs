using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // AppBar colors and styles

        public Color AppBarBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color AppBarForeColor { get; set; } = Color.FromArgb(85, 85, 85); // Dark gray, readable on pastel
        public Color AppBarButtonForeColor { get; set; } = Color.White;
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint

        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Pastel Blue
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(85, 85, 85); // Gray for readability

        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color AppBarLabelBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow

        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(44, 62, 80); // Navy for contrast
        public Color AppBarTitleBackColor { get; set; } = Color.FromArgb(228, 222, 255); // Light Lavender

        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(51, 153, 255); // Soft Blue
        public Color AppBarSubTitleBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Pastel Mint

        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(255, 99, 132); // Candy Red (Strawberry)
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(54, 162, 235); // Soft Blue
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(255, 205, 86); // Candy Yellow

        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS", // Fun and readable; swap to "Montserrat" or "Nunito" if available
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(240, 100, 180), // Candy Pink
            IsUnderlined = false,
            IsStrikeout = false,
            LetterSpacing = 0.2f,
            LineHeight = 1.2f
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 13,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 153, 255), // Soft Blue
            LetterSpacing = 0.1f,
            LineHeight = 1.1f
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(85, 85, 85),
            LetterSpacing = 0f,
            LineHeight = 1.0f
        };

        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(210, 235, 255);   // Pastel Blue
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(204, 255, 240); // Pastel Mint
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}
