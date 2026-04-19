using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SwitchSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
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
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SwitchBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color SwitchForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
    }
}