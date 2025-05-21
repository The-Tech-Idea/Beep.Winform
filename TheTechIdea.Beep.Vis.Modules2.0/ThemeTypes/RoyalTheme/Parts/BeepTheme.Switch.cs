using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle SwitchSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle SwitchUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color SwitchBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color SwitchForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
    }
}