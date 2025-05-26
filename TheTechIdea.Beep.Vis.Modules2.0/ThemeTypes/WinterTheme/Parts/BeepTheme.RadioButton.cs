using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(120, 169, 255);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle
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
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(100, 149, 237);
    }
}