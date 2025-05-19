using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Button Colors and Styles
        // Note: Ensure 'Courier New' font family is available for retro aesthetic. If unavailable, 'Consolas' is a fallback.
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 14f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 14f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(0, 255, 255), // Bright cyan
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(0, 170, 170); // Lighter teal for hover
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for hover text
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for hover border
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for selected border
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal for selected background
        public Color ButtonSelectedForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for selected text
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(0, 170, 170); // Lighter teal for selected hover
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for selected hover text
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for selected hover border
        public Color ButtonBackColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal for button background
        public Color ButtonForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for button text
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for button border
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(139, 0, 0); // Dark red for error background
        public Color ButtonErrorForeColor { get; set; } = Color.FromArgb(255, 85, 85); // Retro red for error text
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(255, 85, 85); // Retro red for error border
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for pressed
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for pressed text
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for pressed border
    }
}