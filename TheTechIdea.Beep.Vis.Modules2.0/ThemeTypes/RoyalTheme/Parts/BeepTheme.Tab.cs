using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = new TypographyStyle
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
        public TypographyStyle TabHoverFont { get; set; } = new TypographyStyle
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
        public TypographyStyle TabSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TabBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color TabForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ActiveTabForeColor { get; set; } = Color.White;
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color TabBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color TabHoverForeColor { get; set; } = Color.White;
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
    }
}