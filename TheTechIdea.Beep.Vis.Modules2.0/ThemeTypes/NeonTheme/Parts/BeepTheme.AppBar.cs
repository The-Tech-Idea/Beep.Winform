using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // AppBar colors and styles
        public Color AppBarBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Darker blue-purple for neon contrast
        public Color AppBarForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for general text
        public Color AppBarButtonForeColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for buttons
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Subtle dark background
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(40, 40, 70); // Darker textbox for contrast
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for textbox text
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple for labels
        public Color AppBarLabelBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Transparent for clean look
        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for title
        public Color AppBarTitleBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Transparent for flexibility
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for subtitle
        public Color AppBarSubTitleBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Transparent for clean design
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(231, 76, 60); // Neon red for close
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for maximize
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for minimize

        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Consistent with card theme
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.3f,
            LetterSpacing = 0.5f,
            FontStyle = FontStyle.Regular,
            
            IsStrikeout = false
        };

        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(46, 204, 113), // Neon green
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
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
            LineHeight = 1.1f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Horizontal; // Horizontal for sleek flow
    }
}