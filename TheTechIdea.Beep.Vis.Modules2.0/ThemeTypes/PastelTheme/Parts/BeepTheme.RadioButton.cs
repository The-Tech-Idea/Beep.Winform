using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // RadioButton properties
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for unchecked background
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for text
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for border
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for checked background
        public Color RadioButtonCheckedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for checked text
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for checked border
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle
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
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle
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
        public Color RadioButtonSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected text
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected background
    }
}