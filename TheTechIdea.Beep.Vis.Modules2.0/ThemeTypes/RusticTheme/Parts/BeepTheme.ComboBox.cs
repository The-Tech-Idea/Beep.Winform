using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // DarkGoldenrod
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(178, 34, 34); // Firebrick
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
    }
}