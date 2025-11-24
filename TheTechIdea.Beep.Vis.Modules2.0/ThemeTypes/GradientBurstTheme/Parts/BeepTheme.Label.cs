using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.White;
        public Color LabelForeColor { get; set; } = Color.Black;
        public Color LabelBorderColor { get; set; } = Color.SteelBlue;
        public Color LabelHoverBorderColor { get; set; } = Color.DeepSkyBlue;
        public Color LabelHoverBackColor { get; set; } = Color.AliceBlue;
        public Color LabelHoverForeColor { get; set; } = Color.Black;
        public Color LabelSelectedBorderColor { get; set; } = Color.OrangeRed;
        public Color LabelSelectedBackColor { get; set; } = Color.Orange;
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.LightGray;
        public Color LabelDisabledForeColor { get; set; } = Color.DarkGray;
        public Color LabelDisabledBorderColor { get; set; } = Color.Gray;

        public TypographyStyle  LabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle  SubLabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Italic);
        public Color SubLabelForColor { get; set; } = Color.DarkSlateGray;
        public Color SubLabelBackColor { get; set; } = Color.WhiteSmoke;
        public Color SubLabelHoverBackColor { get; set; } = Color.LightCyan;
        public Color SubLabelHoverForeColor { get; set; } = Color.Black;
    }
}
