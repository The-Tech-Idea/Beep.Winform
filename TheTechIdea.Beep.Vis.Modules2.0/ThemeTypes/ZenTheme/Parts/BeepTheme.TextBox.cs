using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color TextBoxSelectedForeColor { get; set; } = Color.White;
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(244, 67, 54);
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(255, 235, 235);
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(244, 67, 54);
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(244, 67, 54);
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(200, 100, 100);
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(255, 235, 235);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(244, 67, 54);
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(255, 215, 215);
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(34, 34, 34),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(34, 34, 34),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxSelectedFont { get; set; } = new TypographyStyle
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
    }
}