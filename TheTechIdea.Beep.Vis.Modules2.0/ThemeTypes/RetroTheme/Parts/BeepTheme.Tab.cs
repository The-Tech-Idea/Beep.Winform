using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle TabHoverFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle TabSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public Color TabBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color TabForeColor { get; set; } = Color.White;
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color ActiveTabForeColor { get; set; } = Color.Black;
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color InactiveTabForeColor { get; set; } = Color.White;
        public Color TabBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color TabHoverForeColor { get; set; } = Color.White;
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color TabSelectedForeColor { get; set; } = Color.Black;
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(203, 75, 22);
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
    }
}