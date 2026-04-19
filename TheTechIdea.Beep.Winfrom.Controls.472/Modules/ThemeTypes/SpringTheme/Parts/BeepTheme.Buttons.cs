using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
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
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(0, 100, 0);
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(0, 100, 0);
        public Color ButtonBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color ButtonForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(255, 99, 71);
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(139, 0, 0);
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(0, 100, 0);
    }
}