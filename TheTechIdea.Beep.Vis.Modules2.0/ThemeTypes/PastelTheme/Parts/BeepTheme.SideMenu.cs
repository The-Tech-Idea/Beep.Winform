using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Side Menu Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle SideMenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuTextFont { get; set; } = new TypographyStyle
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
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for menu background
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected
        public Color SideMenuForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for menu text
        public Color SideMenuSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected text
        public Color SideMenuHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for border
        public Color SideMenuTitleTextColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for title
        public Color SideMenuTitleBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for title background
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(100, 100, 100); // Medium gray for subtitle
        public Color SideMenuSubTitleBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for subtitle background
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(220, 215, 230); // Pastel lavender
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for soft pastel effect
    }
}