using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ButtonHoverFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ButtonSelectedFont { get; set; } = new TypographyStyle
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
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color ButtonBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color ButtonForeColor { get; set; } = Color.White;
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(255, 64, 64);
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(192, 0, 0);
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(32, 32, 32);
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
    }
}