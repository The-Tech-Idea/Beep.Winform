using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Label Colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color LabelBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for label background
        public Color LabelForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for label text
        public Color LabelBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for label border
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover border
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for hover background
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover text
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected border
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected background
        public Color LabelSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for selected text
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(200, 200, 205); // Muted gray for disabled background
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(150, 150, 160); // Light gray for disabled text
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(180, 180, 185); // Subtle gray for disabled border
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle
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
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 10f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.1f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for sublabel text
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for sublabel background
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for sublabel hover
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for sublabel hover text
    }
}