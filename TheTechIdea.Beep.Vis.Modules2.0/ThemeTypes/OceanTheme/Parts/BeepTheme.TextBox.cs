using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Textbox colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for background
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for text
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for border
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover border
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover background
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover text
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected border
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected background
        public Color TextBoxSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected text
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(120, 150, 180); // Muted blue for placeholder
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red for error border
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(90, 30, 30); // Dark coral for error background
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red for error text
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red for error text
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(150, 80, 80); // Muted red for error placeholder
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(90, 30, 30); // Dark coral for error textbox
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red for error textbox border
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for error textbox hover
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TextBoxSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}