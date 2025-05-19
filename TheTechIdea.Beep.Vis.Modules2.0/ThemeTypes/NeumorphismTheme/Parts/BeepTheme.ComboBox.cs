using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // ComboBox Colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for background
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for text
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for border
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for hover
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover text
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover border
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected item
        public Color ComboBoxSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for selected text
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected border
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 200, 200); // Light red for error
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red for error text
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        // Note: These CheckBox properties may belong to the CheckBox section
        public Color CheckBoxSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for selected text
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected background
    }
}