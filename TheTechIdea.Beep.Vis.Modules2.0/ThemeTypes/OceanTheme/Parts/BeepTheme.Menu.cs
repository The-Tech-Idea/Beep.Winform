using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Menu Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle MenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color MenuBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for menu background
        public Color MenuForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for menu text
        public Color MenuBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for menu border
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for main item text
        public Color MenuMainItemHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for main item hover text
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for main item hover
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for main item selected text
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for main item selected
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for item text
        public Color MenuItemHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for item hover text
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for item hover
        public Color MenuItemSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for item selected text
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for item selected
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for ocean depth effect
    }
}