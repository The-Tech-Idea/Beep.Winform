using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Textbox colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for background
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for text
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for border
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover border
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for hover background
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover text
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected border
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected background
        public Color TextBoxSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for selected text
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(150, 150, 160); // Light gray for placeholder
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red for error border
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(255, 200, 200); // Light red for error background
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red for error text
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red for error text
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(200, 100, 100); // Muted red for error placeholder
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red for error textbox
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red for error textbox border
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for error textbox hover
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(90, 180, 90), // Soft green
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}