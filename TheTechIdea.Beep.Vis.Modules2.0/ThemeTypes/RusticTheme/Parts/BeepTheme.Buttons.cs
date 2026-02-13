using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(184, 134, 11); // DarkGoldenrod
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
        public Color ButtonSelectedForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        // Default: lighter rustic plate for clearer text
        public Color ButtonBackColor { get; set; } = Color.FromArgb(190, 120, 78); // warm lighter sienna
        public Color ButtonForeColor { get; set; } = Color.FromArgb(32, 20, 12);   // deep brown text
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(178, 34, 34); // Firebrick
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(139, 0, 0); // DarkRed
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(140, 82, 50); // Darker plate
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(245, 235, 225);
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
    }
}
