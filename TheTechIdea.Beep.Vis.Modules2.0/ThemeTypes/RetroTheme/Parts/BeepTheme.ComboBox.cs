using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color ComboBoxForeColor { get; set; } = Color.White;
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color ComboBoxHoverForeColor { get; set; } = Color.White;
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(192, 128, 0);
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 64, 64);
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle
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
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(255, 165, 0);
    }
}