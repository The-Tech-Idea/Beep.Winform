using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(120, 169, 255);
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 99, 99);
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle
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
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(100, 149, 237);
    }
}