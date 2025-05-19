using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Textbox colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for background
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for text
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for border
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover background
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for selected border
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected background
        public Color TextBoxSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected text
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(140, 140, 140); // Slightly lighter gray for placeholder
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(240, 150, 150); // Soft coral for error border
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(240, 180, 180); // Light coral for error background
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(200, 100, 100); // Soft red for error text
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(200, 100, 100); // Soft red for error text
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(200, 130, 130); // Muted coral for error placeholder
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(240, 180, 180); // Light coral for error textbox
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(240, 150, 150); // Soft coral for error textbox border
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for error textbox hover
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
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
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
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
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}