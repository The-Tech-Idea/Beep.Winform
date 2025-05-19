using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // ComboBox Colors and Fonts
        // Note: Ensure 'Courier New' font family is available for retro aesthetic. If unavailable, 'Consolas' is a fallback.
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for background
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for text
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for border
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(0, 170, 170); // Lighter teal for hover
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for hover text
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for hover border
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal for selected item
        public Color ComboBoxSelectedForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for selected text
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for selected border
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(139, 0, 0); // Dark red for error
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(255, 85, 85); // Retro red for error text
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        // Note: These CheckBox properties may belong to the CheckBox section
        public Color CheckBoxSelectedForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for selected text
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal for selected background
    }
}