using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Label Colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color LabelBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for label background
        public Color LabelForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for label text
        public Color LabelBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for label border
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover background
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for selected border
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected background
        public Color LabelSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected text
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(220, 220, 220); // Muted gray for disabled background
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(150, 150, 150); // Light gray for disabled text
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(200, 200, 200); // Subtle gray for disabled border
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle
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
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 10f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.1f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(100, 100, 100); // Medium gray for sublabel text
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for sublabel background
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for sublabel hover
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for sublabel hover text
    }
}