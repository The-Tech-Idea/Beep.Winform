using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Label Colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color LabelBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for label background
        public Color LabelForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for label text
        public Color LabelBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for label border
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover border
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover background
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover text
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected border
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected background
        public Color LabelSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected text
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(50, 80, 110); // Lighter ocean blue for disabled background
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(120, 150, 180); // Muted blue for disabled text
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(80, 110, 140); // Subtle blue for disabled border
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle
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
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 10f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
            LineHeight = 1.1f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for sublabel text
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for sublabel background
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for sublabel hover
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for sublabel hover text
    }
}