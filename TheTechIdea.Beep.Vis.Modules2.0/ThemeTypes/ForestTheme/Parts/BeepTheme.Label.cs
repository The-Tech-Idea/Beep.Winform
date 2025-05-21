using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.FromArgb(30, 45, 20);
        public Color LabelForeColor { get; set; } = Color.FromArgb(220, 230, 210);
        public Color LabelBorderColor { get; set; } = Color.FromArgb(50, 70, 35);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(70, 105, 45);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(45, 65, 28);
        public Color LabelHoverForeColor { get; set; } = Color.White;
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(80, 120, 50);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(60, 90, 40);
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(70, 80, 60);
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(130, 140, 110);
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(60, 70, 50);
        public TypographyStyle LabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle SubLabelFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Italic);
        public Color SubLabelForColor { get; set; } = Color.FromArgb(180, 200, 160);
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(35, 50, 22);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(55, 80, 30);
        public Color SubLabelHoverForeColor { get; set; } = Color.WhiteSmoke;
    }
}
