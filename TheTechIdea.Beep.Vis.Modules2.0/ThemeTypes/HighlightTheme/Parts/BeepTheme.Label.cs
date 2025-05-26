using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Label Colors and Fonts
//<<<<<<< HEAD
        public Color LabelBackColor { get; set; } = Color.White;
        public Color LabelForeColor { get; set; } = Color.Black;
        public Color LabelBorderColor { get; set; } = Color.FromArgb(255, 220, 160);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(255, 200, 140);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(255, 245, 210);
        public Color LabelHoverForeColor { get; set; } = Color.Black;
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(255, 180, 110);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(255, 230, 180);
        public Color LabelSelectedForeColor { get; set; } = Color.Black;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color LabelDisabledForeColor { get; set; } = Color.Gray;
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(220, 220, 220);
        public TypographyStyle  LabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public TypographyStyle  SubLabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8, FontStyle.Italic);
        public Color SubLabelForColor { get; set; } = Color.Gray;
        public Color SubLabelBackColor { get; set; } =Color.FromArgb(255, 255, 204);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(255, 245, 210);
        public Color SubLabelHoverForeColor { get; set; } = Color.Black;
    }
}
