using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle NavigationTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle NavigationSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle NavigationUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color NavigationBackColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color NavigationForeColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(45, 45, 128); // Darker royal blue
        public Color NavigationHoverForeColor { get; set; } = Color.White;
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}