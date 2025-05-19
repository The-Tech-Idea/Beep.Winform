using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Button Colors and Styles
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(50, 80, 110); // Lighter ocean blue for hover
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for hover text
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover border
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected border
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(20, 50, 80); // Darker blue for selected background
        public Color ButtonSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected text
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for selected hover
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected hover text
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for selected hover border
        public Color ButtonBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for button background
        public Color ButtonForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for button text
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for button border
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(90, 30, 30); // Dark coral for error background
        public Color ButtonErrorForeColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red for error text
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red for error border
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for pressed
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for pressed text
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for pressed border
    }
}