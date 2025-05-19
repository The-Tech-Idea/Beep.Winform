using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color RadioButtonForeColor { get; set; } = Color.White;
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color RadioButtonHoverForeColor { get; set; } = Color.White;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle
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
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
    }
}