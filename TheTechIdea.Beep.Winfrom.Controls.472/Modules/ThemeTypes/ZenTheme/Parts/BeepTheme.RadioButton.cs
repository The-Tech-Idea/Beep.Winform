using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(34, 34, 34),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle
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
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(76, 175, 80);
    }
}