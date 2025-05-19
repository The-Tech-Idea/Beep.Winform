using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Label Colors and Fonts

        // Default label: pastel pink, navy text, mint border
        public Color LabelBackColor { get; set; } = Color.FromArgb(255, 224, 235);      // Pastel Pink
        public Color LabelForeColor { get; set; } = Color.FromArgb(44, 62, 80);         // Navy
        public Color LabelBorderColor { get; set; } = Color.FromArgb(127, 255, 212);    // Mint

        // Hover: lemon yellow with candy pink border, navy text
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        // Selected: pastel blue with candy pink border, candy pink text
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Pastel Blue
        public Color LabelSelectedForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        // Disabled: pale gray, muted gray text/border
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(240, 240, 240);   // Very light gray
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(180, 180, 180);   // Gray
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(210, 210, 210); // Muted border

        // Fonts: playful main, lighter sublabel
        public TypographyStyle LabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
        public TypographyStyle SubLabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Italic);

        // SubLabel: mint on pink, lemon highlight on hover
        public Color SubLabelForColor { get; set; } = Color.FromArgb(127, 255, 212);         // Mint
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(255, 224, 235);        // Pastel Pink
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(255, 253, 194);   // Lemon Yellow
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180);   // Candy Pink
    }
}
