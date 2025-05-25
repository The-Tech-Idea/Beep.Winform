using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SwitchSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
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
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SwitchBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color SwitchForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color SwitchSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
    }
}