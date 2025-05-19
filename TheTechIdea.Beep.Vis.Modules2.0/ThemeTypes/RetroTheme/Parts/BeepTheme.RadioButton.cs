using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color RadioButtonForeColor { get; set; } = Color.White;
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.Black;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(203, 75, 22);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color RadioButtonHoverForeColor { get; set; } = Color.White;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public Color RadioButtonSelectedForeColor { get; set; } = Color.Black;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
    }
}