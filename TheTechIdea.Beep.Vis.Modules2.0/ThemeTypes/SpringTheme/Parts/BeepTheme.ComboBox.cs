using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 99, 71);
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle
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
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
    }
}