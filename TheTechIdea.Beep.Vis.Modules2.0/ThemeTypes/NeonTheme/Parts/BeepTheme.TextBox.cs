using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Textbox colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for background
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for text
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for border
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover border
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover background
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected border
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected background
        public Color TextBoxSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected text
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(100, 100, 120); // Muted gray-blue for placeholder
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(231, 76, 60); // Neon red for error border
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(70, 30, 30); // Dark red-gray for error background
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(231, 76, 60); // Neon red for error text
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(231, 76, 60); // Neon red for error text
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(150, 50, 50); // Muted red for error placeholder
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(231, 76, 60); // Neon red for error textbox
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(231, 76, 60); // Neon red for error textbox border
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for error textbox hover
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
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
            TextColor = Color.FromArgb(241, 196, 15), // Neon yellow
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
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}