using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle SwitchSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle SwitchUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color SwitchBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color SwitchForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(34, 34, 34);
    }
}