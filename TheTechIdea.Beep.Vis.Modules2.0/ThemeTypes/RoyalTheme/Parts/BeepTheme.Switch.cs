using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle SwitchSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle SwitchUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color SwitchBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color SwitchForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color SwitchHoverForeColor { get; set; } = Color.White;
    }
}