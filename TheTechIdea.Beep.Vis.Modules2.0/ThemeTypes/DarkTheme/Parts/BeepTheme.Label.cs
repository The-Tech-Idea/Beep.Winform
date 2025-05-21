using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color LabelForeColor { get; set; } = Color.LightGray;
        public Color LabelBorderColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color LabelHoverForeColor { get; set; } = Color.White;
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(70, 130, 180);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(70, 130, 180);
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(25, 25, 25);
        public Color LabelDisabledForeColor { get; set; } = Color.Gray;
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(50, 50, 50);
        public TypographyStyle LabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle SubLabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8F, FontStyle.Italic);
        public Color SubLabelForColor { get; set; } = Color.Gray;
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color SubLabelHoverForeColor { get; set; } = Color.LightGray;
    }
}
