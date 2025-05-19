using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // AppBar colors and styles
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color AppBarBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for neumorphic background
        public Color AppBarForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for general text
        public Color AppBarButtonForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for button text
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for button background
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(220, 220, 225); // Slightly darker gray for textbox
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for textbox text
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for labels
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent; // Transparent for labels
        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for title
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent; // Transparent for title background
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for subtitle
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent; // Transparent for subtitle background
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red for close button
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for maximize button
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(255, 180, 90); // Soft orange for minimize button
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(240, 240, 245); // Light gray gradient start
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(220, 220, 225); // Slightly darker gray gradient end
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(230, 230, 235); // Mid-tone gray for gradient
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for soft neumorphic effect
    }
}