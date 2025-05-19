using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // AppBar colors and styles
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color AppBarBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for ocean depth
        public Color AppBarForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for text
        public Color AppBarButtonForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for button text
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(20, 50, 80); // Darker blue for buttons
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for textbox
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for textbox text
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for labels
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent; // Transparent for labels
        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for title
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent; // Transparent for title background
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for subtitle
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent; // Transparent for subtitle background
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red for close button
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for maximize button
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for minimize button
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
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
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for ocean depth effect
    }
}