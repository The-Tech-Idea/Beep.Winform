using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle
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
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle
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
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
    }
}