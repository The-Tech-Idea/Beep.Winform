using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color TextBoxForeColor { get; set; } = Color.White;
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color TextBoxHoverForeColor { get; set; } = Color.White;
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color TextBoxSelectedForeColor { get; set; } = Color.White;
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(255, 77, 77);
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(255, 77, 77);
        public Color TextBoxErrorForeColor { get; set; } = Color.White;
        public Color TextBoxErrorTextColor { get; set; } = Color.White;
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(255, 99, 99);
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(255, 77, 77);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(255, 99, 99);
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(255, 99, 99);
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxSelectedFont { get; set; } = new TypographyStyle
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
    }
}