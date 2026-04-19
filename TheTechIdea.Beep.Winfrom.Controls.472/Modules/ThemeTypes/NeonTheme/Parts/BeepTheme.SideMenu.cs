using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Side Menu Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle SideMenuTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.4f,
            LetterSpacing = 0.5f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(46, 204, 113), // Neon green
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SideMenuTextFont { get; set; } = new TypographyStyle
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
        public Color SideMenuBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for menu background
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected
        public Color SideMenuForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for menu text
        public Color SideMenuSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected text
        public Color SideMenuHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for border
        public Color SideMenuTitleTextColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for title
        public Color SideMenuTitleBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for title background
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.4f,
            LetterSpacing = 0.5f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for subtitle
        public Color SideMenuSubTitleBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for subtitle background
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(46, 204, 113), // Neon green
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for sleek flow
    }
}