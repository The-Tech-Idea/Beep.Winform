using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Button Colors and Styles
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(90, 180, 90), // Soft green
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for hover
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover text
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover border
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for selected border
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for selected background
        public Color ButtonSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for selected text
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(190, 190, 195); // Darker gray for selected hover
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected hover text
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected hover border
        public Color ButtonBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for button background
        public Color ButtonForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for button text
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for button border
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(255, 200, 200); // Light red for error background
        public Color ButtonErrorForeColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red for error text
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red for error border
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for pressed
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for pressed text
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for pressed border
    }
}