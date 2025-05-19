using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // ComboBox Colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for background
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for text
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for border
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover border
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected item
        public Color ComboBoxSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected text
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected border
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(70, 30, 30); // Dark red-gray for error
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(231, 76, 60); // Neon red for error text
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        // Note: These CheckBox properties may belong to the CheckBox section
        public Color CheckBoxSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected text
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple for selected background
    }
}