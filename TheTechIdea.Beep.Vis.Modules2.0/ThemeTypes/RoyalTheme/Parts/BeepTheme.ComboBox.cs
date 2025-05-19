using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color ComboBoxForeColor { get; set; } = Color.White;
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color ComboBoxHoverForeColor { get; set; } = Color.White;
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 77, 77);
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle
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
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
    }
}