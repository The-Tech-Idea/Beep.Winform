using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle NavigationTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(34, 34, 34),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle NavigationSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle NavigationUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color NavigationBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color NavigationForeColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color NavigationHoverForeColor { get; set; } = Color.White;
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}