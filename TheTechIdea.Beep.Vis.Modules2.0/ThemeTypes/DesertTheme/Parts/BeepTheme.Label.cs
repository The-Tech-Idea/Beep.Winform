using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.FromArgb(255, 245, 230);          // Light sand
        public Color LabelForeColor { get; set; } = Color.FromArgb(92, 64, 51);             // Dark brown
        public Color LabelBorderColor { get; set; } = Color.FromArgb(210, 180, 140);        // Tan
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(198, 134, 66);    // Desert orange
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(255, 250, 240);     // Off-white
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(102, 71, 49);       // Medium brown
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(153, 102, 51); // Rich brown
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(244, 164, 96);   // Warm orange
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(240, 230, 210);  // Pale sand
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(180, 160, 140);  // Muted brown
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(210, 180, 140);

        public TypographyStyle LabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle SubLabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Italic);
        public Color SubLabelForColor { get; set; } = Color.FromArgb(92, 64, 51);
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(255, 245, 230);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(255, 250, 240);
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(102, 71, 49);
    }
}
