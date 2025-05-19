using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // RadioButton properties
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for unchecked background
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for text
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for border
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for checked background
        public Color RadioButtonCheckedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for checked text
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for checked border
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover border
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle
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
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle
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
        public Color RadioButtonSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected text
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple for selected background
    }
}