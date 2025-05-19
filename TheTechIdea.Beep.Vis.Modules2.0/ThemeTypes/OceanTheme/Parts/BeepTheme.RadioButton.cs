using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // RadioButton properties
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for unchecked background
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for text
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for border
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for checked background
        public Color RadioButtonCheckedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for checked text
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for checked border
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover text
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover border
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle
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
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle
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
        public Color RadioButtonSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected text
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected background
    }
}