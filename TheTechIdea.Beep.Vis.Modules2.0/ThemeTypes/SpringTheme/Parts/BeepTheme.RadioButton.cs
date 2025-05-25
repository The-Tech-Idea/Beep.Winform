using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle
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
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
    }
}