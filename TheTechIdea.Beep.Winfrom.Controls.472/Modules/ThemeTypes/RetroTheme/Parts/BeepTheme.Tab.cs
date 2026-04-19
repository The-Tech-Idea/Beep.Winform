using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TabHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TabSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TabBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color TabForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color ActiveTabForeColor { get; set; } = Color.White;
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color TabBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color TabHoverForeColor { get; set; } = Color.White;
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(192, 128, 0);
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
    }
}