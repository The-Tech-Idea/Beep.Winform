using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // CheckBox properties
        // Note: Ensure 'Courier New' font family is available for retro aesthetic. If unavailable, 'Consolas' is a fallback.
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for unchecked background
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for text
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for border
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal for checked background
        public Color CheckBoxCheckedForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for checked text
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for checked border
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(0, 170, 170); // Lighter teal for hover
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for hover text
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for hover border
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle
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
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}