using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(178, 34, 34); // Crimson
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
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
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
    }
}