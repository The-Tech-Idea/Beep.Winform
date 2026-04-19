using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(120, 169, 255);
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color TextBoxSelectedForeColor { get; set; } = Color.White;
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(150, 170, 190);
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(255, 99, 99);
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(255, 240, 240);
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(255, 99, 99);
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(255, 99, 99);
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(200, 150, 150);
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(255, 240, 240);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(255, 99, 99);
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(255, 220, 220);
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}