using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SwitchSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
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
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SwitchBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color SwitchForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(120, 169, 255);
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(27, 62, 92);
    }
}