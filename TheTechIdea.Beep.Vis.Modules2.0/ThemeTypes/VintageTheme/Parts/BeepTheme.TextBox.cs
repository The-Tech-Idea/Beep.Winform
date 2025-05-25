using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color TextBoxSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(150, 120, 100);
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(128, 0, 0);
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(128, 0, 0);
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(180, 80, 80);
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(128, 0, 0);
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(139, 0, 0);
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}