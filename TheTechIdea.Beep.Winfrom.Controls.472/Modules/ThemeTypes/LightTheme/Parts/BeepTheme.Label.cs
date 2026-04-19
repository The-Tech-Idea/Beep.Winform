using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } =Color.White;
        public Color LabelForeColor { get; set; } = Color.Black;
        public Color LabelBorderColor { get; set; } = Color.LightGray;
        public Color LabelHoverBorderColor { get; set; } = Color.DodgerBlue;
        public Color LabelHoverBackColor { get; set; } = Color.LightBlue;
        public Color LabelHoverForeColor { get; set; } = Color.Black;
        public Color LabelSelectedBorderColor { get; set; } = Color.RoyalBlue;
        public Color LabelSelectedBackColor { get; set; } = Color.LightSkyBlue;
        public Color LabelSelectedForeColor { get; set; } = Color.Black;
        public Color LabelDisabledBackColor { get; set; } = Color.LightGray;
        public Color LabelDisabledForeColor { get; set; } = Color.Gray;
        public Color LabelDisabledBorderColor { get; set; } = Color.Gainsboro;
        public TypographyStyle  LabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  SubLabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8F, FontStyle.Italic);
        public Color SubLabelForColor { get; set; } = Color.DarkSlateGray;
        public Color SubLabelBackColor { get; set; } =Color.White;
        public Color SubLabelHoverBackColor { get; set; } = Color.LightCyan;
        public Color SubLabelHoverForeColor { get; set; } = Color.DarkSlateBlue;
    }
}
