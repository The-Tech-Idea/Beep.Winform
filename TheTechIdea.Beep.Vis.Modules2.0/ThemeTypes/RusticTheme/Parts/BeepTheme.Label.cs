using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color LabelForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color LabelBorderColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(227, 220, 203);
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(150, 140, 120);
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(200, 190, 170);
        public TypographyStyle LabelFont { get; set; }
        public TypographyStyle SubLabelFont { get; set; }
        public Color SubLabelForColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
    }
}
