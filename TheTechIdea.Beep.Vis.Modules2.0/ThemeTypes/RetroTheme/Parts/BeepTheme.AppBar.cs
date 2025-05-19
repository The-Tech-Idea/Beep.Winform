using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // AppBar colors and styles
        // Note: Ensure 'Courier New' font family is available for retro aesthetic. If unavailable, 'Consolas' is a fallback.
        public Color AppBarBackColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal for background
        public Color AppBarForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for text
        public Color AppBarButtonForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for button text
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal for buttons
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for textbox
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for textbox text
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for labels
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent; // Transparent for labels
        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for title
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent; // Transparent for title background
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(192, 192, 192); // Light gray for subtitle
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent; // Transparent for subtitle background
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(255, 85, 85); // Retro red for close button
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for maximize button
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for minimize button
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(255, 215, 0), // Retro yellow
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 14f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(192, 192, 192), // Light gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(0, 43, 43); // Darker teal
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(0, 64, 64); // Mid-tone teal
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for retro CRT effect
    }
}