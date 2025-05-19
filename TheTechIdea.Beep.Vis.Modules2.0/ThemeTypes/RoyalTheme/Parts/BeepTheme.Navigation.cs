using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle NavigationTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle NavigationSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle NavigationUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color NavigationBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color NavigationForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color NavigationHoverForeColor { get; set; } = Color.White;
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}