using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color CheckBoxCheckedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle
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
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle
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
    }
}