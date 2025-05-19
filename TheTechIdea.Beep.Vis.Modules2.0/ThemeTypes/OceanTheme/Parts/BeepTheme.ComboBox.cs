using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // ComboBox Colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for background
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for text
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for border
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover text
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover border
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected item
        public Color ComboBoxSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected text
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected border
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(90, 30, 30); // Dark coral for error
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red for error text
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        // Note: These CheckBox properties may belong to the CheckBox section
        public Color CheckBoxSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected text
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected background
    }
}