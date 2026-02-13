using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(90, 90, 90);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color ButtonBackColor { get; set; } = Color.FromArgb(220, 224, 228); // light zen plate
        public Color ButtonForeColor { get; set; } = Color.FromArgb(40, 45, 50);
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(120, 150, 200);
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(244, 67, 54);
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(200, 40, 30);
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(190, 198, 206);
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(35, 40, 45);
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(120, 150, 200);
    }
}
