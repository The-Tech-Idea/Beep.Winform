using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color CheckBoxForeColor { get; set; } = Color.White;
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(120, 169, 255);
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(60, 100, 140);
        public Color CheckBoxHoverForeColor { get; set; } = Color.White;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
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
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}