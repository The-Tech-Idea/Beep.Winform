using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color CheckBoxForeColor { get; set; } = Color.White;
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color CheckBoxHoverForeColor { get; set; } = Color.White;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle
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
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle
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
    }
}