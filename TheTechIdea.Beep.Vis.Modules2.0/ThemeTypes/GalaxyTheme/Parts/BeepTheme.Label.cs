using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color LabelForeColor { get; set; } = Color.White;
        public Color LabelBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Subtle border
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Light blue
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Hover background
        public Color LabelHoverForeColor { get; set; } = Color.White;
        public Color LabelSelectedBorderColor { get; set; } = Color.White;
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(0x20, 0x20, 0x30);
        public Color LabelDisabledForeColor { get; set; } = Color.Gray;
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33);
        public TypographyStyle  LabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  SubLabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Italic);
        public Color SubLabelForColor { get; set; } = Color.FromArgb(0xA0, 0xA0, 0xFF); // Soft violet
        public Color SubLabelBackColor { get; set; } =Color.FromArgb(10, 10, 30);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(0x2A, 0x2A, 0x50);
        public Color SubLabelHoverForeColor { get; set; } = Color.White;
    }
}
