using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // CheckBox properties
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for unchecked background
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for text
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for border
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for checked background
        public Color CheckBoxCheckedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for checked text
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for checked border
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for hover
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover text
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover border
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle
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
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}