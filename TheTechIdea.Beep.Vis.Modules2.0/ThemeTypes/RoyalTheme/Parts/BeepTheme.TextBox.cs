using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color TextBoxSelectedForeColor { get; set; } = Color.White;
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(150, 150, 170); // Muted silver
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(178, 34, 34); // Crimson
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(255, 230, 230); // Light red
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(178, 34, 34); // Crimson
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(200, 100, 100); // Muted red
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(255, 230, 230); // Light red
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(178, 34, 34); // Crimson
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(255, 200, 200); // Brighter red
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}