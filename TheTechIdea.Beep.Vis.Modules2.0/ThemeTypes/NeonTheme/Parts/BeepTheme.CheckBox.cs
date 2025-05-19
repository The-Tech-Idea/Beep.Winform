using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // CheckBox properties
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for unchecked background
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for text
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for border
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for checked background
        public Color CheckBoxCheckedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for checked text contrast
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for checked border
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover border
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle
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
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle
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
    }
}