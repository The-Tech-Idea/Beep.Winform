using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // ComboBox Colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for background
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for text
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for border
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected item
        public Color ComboBoxSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected text
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for selected border
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(240, 180, 180); // Light coral for error
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(200, 100, 100); // Soft red for error text
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle
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
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        // Note: These CheckBox properties may belong to the CheckBox section
        public Color CheckBoxSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected text
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for selected background
    }
}