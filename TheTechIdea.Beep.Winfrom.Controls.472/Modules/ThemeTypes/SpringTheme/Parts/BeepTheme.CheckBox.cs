using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle
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
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}