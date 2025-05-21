using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color TextBoxForeColor { get; set; } = Color.White;
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color TextBoxHoverForeColor { get; set; } = Color.White;
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color TextBoxSelectedForeColor { get; set; } = Color.White;
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(255, 64, 64);
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(64, 48, 48);
        public Color TextBoxErrorForeColor { get; set; } = Color.White;
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(255, 64, 64);
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(192, 128, 128);
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(64, 48, 48);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(255, 64, 64);
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(96, 64, 64);
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle
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
        public TypographyStyle TextBoxHoverFont { get; set; } = new TypographyStyle
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
        public TypographyStyle TextBoxSelectedFont { get; set; } = new TypographyStyle
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