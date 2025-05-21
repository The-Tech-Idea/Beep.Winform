using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color RadioButtonForeColor { get; set; } = Color.White;
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(192, 128, 0);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color RadioButtonHoverForeColor { get; set; } = Color.White;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(255, 165, 0);
    }
}