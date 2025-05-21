using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Label Colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color LabelBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for label background
        public Color LabelForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for label text
        public Color LabelBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for label border
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover border
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover background
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected border
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected background
        public Color LabelSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected text
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(60, 60, 80); // Muted gray for disabled background
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(100, 100, 120); // Muted gray-blue for disabled text
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(80, 80, 100); // Subtle gray for disabled border
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle
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
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 10f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(155, 89, 182), // Neon purple
            LineHeight = 1.1f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple for sublabel text
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for sublabel background
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for sublabel hover
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for sublabel hover text
    }
}