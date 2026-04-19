using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ComboBoxSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CheckBoxSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
    }
}