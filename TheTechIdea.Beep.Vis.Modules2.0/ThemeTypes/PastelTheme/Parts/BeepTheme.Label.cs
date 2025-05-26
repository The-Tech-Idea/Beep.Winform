using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } =Color.FromArgb(245, 183, 203);
        public Color LabelForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color LabelBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(180, 180, 180);
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle() { FontSize = 10, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color SubLabelBackColor { get; set; } =Color.FromArgb(245, 183, 203);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
    }
}