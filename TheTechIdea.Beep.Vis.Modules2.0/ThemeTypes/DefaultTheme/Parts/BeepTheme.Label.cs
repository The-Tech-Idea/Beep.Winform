using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.Transparent;
        public Color LabelForeColor { get; set; } = Color.Black;
        public Color LabelBorderColor { get; set; } = Color.LightGray;
        public Color LabelHoverBorderColor { get; set; } = Color.SteelBlue;
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(230, 240, 255);
        public Color LabelHoverForeColor { get; set; } = Color.Black;
        public Color LabelSelectedBorderColor { get; set; } = Color.DodgerBlue;
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(200, 220, 255);
        public Color LabelSelectedForeColor { get; set; } = Color.Black;
        public Color LabelDisabledBackColor { get; set; } = Color.LightGray;
        public Color LabelDisabledForeColor { get; set; } = Color.Gray;
        public Color LabelDisabledBorderColor { get; set; } = Color.Gray;
        public TypographyStyle LabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle SubLabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Italic);
        public Color SubLabelForColor { get; set; } = Color.DarkSlateGray;
        public Color SubLabelBackColor { get; set; } = Color.Transparent;
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color SubLabelHoverForeColor { get; set; } = Color.DodgerBlue;
    }
}
