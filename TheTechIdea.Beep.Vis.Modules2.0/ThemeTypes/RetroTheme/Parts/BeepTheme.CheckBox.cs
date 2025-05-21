using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color CheckBoxForeColor { get; set; } = Color.White;
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(192, 128, 0);
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color CheckBoxHoverForeColor { get; set; } = Color.White;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle
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
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}