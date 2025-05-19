using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Side Menu Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle SideMenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
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
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for menu background
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected
        public Color SideMenuForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for menu text
        public Color SideMenuSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected text
        public Color SideMenuHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover text
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for border
        public Color SideMenuTitleTextColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for title
        public Color SideMenuTitleBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for title background
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for subtitle
        public Color SideMenuSubTitleBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for subtitle background
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for ocean depth effect
    }
}