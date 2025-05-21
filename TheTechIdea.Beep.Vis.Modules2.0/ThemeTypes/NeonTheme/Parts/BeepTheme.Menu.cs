using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Menu Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle MenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MenuItemSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
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
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color MenuBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for menu background
        public Color MenuForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for menu text
        public Color MenuBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for menu border
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for main item text
        public Color MenuMainItemHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for main item hover text
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for main item hover
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for main item selected text
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for main item selected
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for item text
        public Color MenuItemHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for item hover text
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for item hover
        public Color MenuItemSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for item selected text
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for item selected
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for sleek flow
    }
}