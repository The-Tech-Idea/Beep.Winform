using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color TextBoxSelectedForeColor { get; set; } = Color.White;
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(139, 0, 0);
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(255, 99, 71);
        public Color TextBoxErrorForeColor { get; set; } = Color.White;
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(139, 0, 0);
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(180, 80, 80);
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(255, 99, 71);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(139, 0, 0);
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(220, 20, 60);
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
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