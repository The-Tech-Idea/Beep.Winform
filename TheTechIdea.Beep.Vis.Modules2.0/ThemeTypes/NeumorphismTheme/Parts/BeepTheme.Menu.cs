using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Menu Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle MenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color MenuBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for menu background
        public Color MenuForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for menu text
        public Color MenuBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for menu border
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for main item text
        public Color MenuMainItemHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for main item hover text
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for main item hover
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for main item selected text
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for main item selected
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for item text
        public Color MenuItemHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for item hover text
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for item hover
        public Color MenuItemSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for item selected text
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for item selected
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(240, 240, 245); // Light gray gradient start
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(210, 210, 215); // Darker gray gradient end
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(230, 230, 235); // Mid-tone gray for gradient
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for soft neumorphic effect
    }
}