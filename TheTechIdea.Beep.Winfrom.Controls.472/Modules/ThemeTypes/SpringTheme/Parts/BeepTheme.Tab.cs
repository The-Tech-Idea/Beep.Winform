using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TabHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TabSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TabBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color TabForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color ActiveTabForeColor { get; set; } = Color.White;
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color TabBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
    }
}