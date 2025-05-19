using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Menu Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle MenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MenuItemSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color MenuBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for menu background
        public Color MenuForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for menu text
        public Color MenuBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for menu border
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for main item text
        public Color MenuMainItemHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for main item hover text
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for main item hover
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for main item selected text
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for main item selected
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for item text
        public Color MenuItemHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for item hover text
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for item hover
        public Color MenuItemSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for item selected text
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for item selected
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(220, 215, 230); // Pastel lavender
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for soft pastel effect
    }
}