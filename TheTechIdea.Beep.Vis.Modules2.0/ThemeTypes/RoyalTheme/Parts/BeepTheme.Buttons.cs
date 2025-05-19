using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ButtonBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color ButtonForeColor { get; set; } = Color.White;
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(255, 77, 77);
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(255, 99, 99);
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(22, 26, 30);
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
    }
}