using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.White;
        public Color LabelForeColor { get; set; } = Color.Black;
        public Color LabelBorderColor { get; set; } = Color.DarkGray;
        public Color LabelHoverBorderColor { get; set; } = Color.Black;
        public Color LabelHoverBackColor { get; set; } = Color.LightGray;
        public Color LabelHoverForeColor { get; set; } = Color.Black;
        public Color LabelSelectedBorderColor { get; set; } = Color.Black;
        public Color LabelSelectedBackColor { get; set; } = Color.DimGray;
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.LightGray;
        public Color LabelDisabledForeColor { get; set; } = Color.Gray;
        public Color LabelDisabledBorderColor { get; set; } = Color.Gray;
        public TypographyStyle LabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle SubLabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8F, FontStyle.Italic);
        public Color SubLabelForColor { get; set; } = Color.DarkGray;
        public Color SubLabelBackColor { get; set; } = Color.White;
        public Color SubLabelHoverBackColor { get; set; } = Color.LightGray;
        public Color SubLabelHoverForeColor { get; set; } = Color.Black;
    }
}
