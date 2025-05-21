using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Label Colors and Fonts

        public Color LabelBackColor { get; set; } = Color.FromArgb(18, 18, 32);           // Cyberpunk Black
        public Color LabelForeColor { get; set; } = Color.FromArgb(0, 255, 255);          // Neon Cyan
        public Color LabelBorderColor { get; set; } = Color.FromArgb(255, 0, 255);        // Neon Magenta

        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(255, 255, 0);   // Neon Yellow
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(0, 255, 255);     // Neon Cyan
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);     // Neon Yellow

        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(0, 255, 128);// Neon Green
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(255, 0, 255);  // Neon Magenta
        public Color LabelSelectedForeColor { get; set; } = Color.White;

        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(48, 48, 64);   // Dimmed cyberpunk
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(128, 128, 160);// Muted neon blue
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(80, 80, 128);// Muted magenta

        public TypographyStyle LabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Regular);
        public TypographyStyle SubLabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 10f, FontStyle.Italic);

        public Color SubLabelForColor { get; set; } = Color.FromArgb(0, 255, 128);        // Neon Green
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(34, 34, 68);        // Cyberpunk Panel

        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(0, 255, 255);  // Neon Cyan
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);  // Neon Yellow
    }
}
