using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // CheckBox properties
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for unchecked background
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for text
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for border
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for checked background
        public Color CheckBoxCheckedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for checked text
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for checked border
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for hover text
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for hover border
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle
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
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle
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
    }
}