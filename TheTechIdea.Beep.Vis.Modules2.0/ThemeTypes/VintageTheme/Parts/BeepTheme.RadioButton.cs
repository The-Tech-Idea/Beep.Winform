using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle
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
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color RadioButtonSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
    }
}