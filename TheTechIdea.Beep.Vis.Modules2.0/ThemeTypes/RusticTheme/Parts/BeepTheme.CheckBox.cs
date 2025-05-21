using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // DarkGoldenrod
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle
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
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
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