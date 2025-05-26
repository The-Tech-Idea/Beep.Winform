using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Label Colors and Fonts
//<<<<<<< HEAD
        public Color LabelBackColor { get; set; } = Color.Black;
        public Color LabelForeColor { get; set; } = Color.White;
        public Color LabelBorderColor { get; set; } = Color.White;
        public Color LabelHoverBorderColor { get; set; } = Color.Yellow;
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color LabelHoverForeColor { get; set; } = Color.Yellow;
        public Color LabelSelectedBorderColor { get; set; } = Color.Cyan;
        public Color LabelSelectedBackColor { get; set; } = Color.White;
        public Color LabelSelectedForeColor { get; set; } = Color.Black;
        public Color LabelDisabledBackColor { get; set; } = Color.Gray;
        public Color LabelDisabledForeColor { get; set; } = Color.DarkGray;
        public Color LabelDisabledBorderColor { get; set; } = Color.DimGray;
        public TypographyStyle  LabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle  SubLabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public Color SubLabelForColor { get; set; } = Color.LightGray;
        public Color SubLabelBackColor { get; set; } = Color.Black;
        public Color SubLabelHoverBackColor { get; set; } = Color.DarkGray;
        public Color SubLabelHoverForeColor { get; set; } = Color.Yellow;
    }
}
