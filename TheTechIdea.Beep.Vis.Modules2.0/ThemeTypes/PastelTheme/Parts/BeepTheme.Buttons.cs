using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Button Colors and Styles
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
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
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
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
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for selected border
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected background
        public Color ButtonSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected text
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(190, 210, 230); // Slightly darker pastel blue for selected hover
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for selected hover text
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for selected hover border
        public Color ButtonBackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for button background
        public Color ButtonForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for button text
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for button border
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(240, 180, 180); // Light coral for error background
        public Color ButtonErrorForeColor { get; set; } = Color.FromArgb(200, 100, 100); // Soft red for error text
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(200, 100, 100); // Soft red for error border
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for pressed
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for pressed text
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for pressed border
    }
}