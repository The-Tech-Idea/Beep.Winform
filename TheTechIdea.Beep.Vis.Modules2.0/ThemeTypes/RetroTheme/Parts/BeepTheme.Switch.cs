using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle SwitchSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle SwitchUnSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color SwitchBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color SwitchForeColor { get; set; } = Color.White;
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(203, 75, 22);
        public Color SwitchSelectedForeColor { get; set; } = Color.Black;
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
        public Color SwitchHoverForeColor { get; set; } = Color.White;
    }
}