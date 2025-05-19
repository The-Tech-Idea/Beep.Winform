using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // AppBar colors and styles
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color AppBarBackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for background
        public Color AppBarForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for text
        public Color AppBarButtonForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for button text
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for buttons
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for textbox
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for textbox text
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(100, 100, 100); // Medium gray for labels
        public Color AppBarLabelBackColor { get; set; } = Color.Transparent; // Transparent for labels
        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for title
        public Color AppBarTitleBackColor { get; set; } = Color.Transparent; // Transparent for title background
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(140, 140, 140); // Slightly lighter gray for subtitle
        public Color AppBarSubTitleBackColor { get; set; } = Color.Transparent; // Transparent for subtitle background
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(240, 150, 150); // Soft coral for close button
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for maximize button
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(200, 200, 240); // Pastel lavender for minimize button
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
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
            TextColor = Color.FromArgb(140, 140, 140), // Slightly lighter gray
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
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(220, 215, 230); // Pastel lavender
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for soft pastel effect
    }
}