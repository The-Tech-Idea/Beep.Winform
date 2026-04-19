using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color LabelForeColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color LabelBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color LabelHoverForeColor { get; set; } = Color.White;
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(200, 210, 220);
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(150, 160, 170);
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(180, 190, 200);
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 80, 120) };
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 105, 148) };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color SubLabelHoverForeColor { get; set; } = Color.White;
    }
}
