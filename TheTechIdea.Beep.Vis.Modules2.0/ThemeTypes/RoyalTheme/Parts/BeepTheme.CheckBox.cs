using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle
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
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}