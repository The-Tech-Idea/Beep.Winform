using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color CheckBoxForeColor { get; set; } = Color.White;
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color CheckBoxHoverForeColor { get; set; } = Color.White;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle
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
    }
}