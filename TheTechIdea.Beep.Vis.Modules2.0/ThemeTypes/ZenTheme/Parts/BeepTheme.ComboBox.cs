using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(244, 67, 54);
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle
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
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(76, 175, 80);
    }
}