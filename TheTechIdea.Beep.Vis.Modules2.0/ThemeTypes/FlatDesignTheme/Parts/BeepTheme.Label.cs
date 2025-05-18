using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.Transparent;
        public Color LabelForeColor { get; set; } = Color.Black;
        public Color LabelBorderColor { get; set; } = Color.LightGray;
        public Color LabelHoverBorderColor { get; set; } = Color.Gray;
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color LabelHoverForeColor { get; set; } = Color.Black;
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(204, 228, 247);
        public Color LabelSelectedForeColor { get; set; } = Color.Black;
        public Color LabelDisabledBackColor { get; set; } = Color.LightGray;
        public Color LabelDisabledForeColor { get; set; } = Color.Gray;
        public Color LabelDisabledBorderColor { get; set; } = Color.Gray;

        public Font LabelFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
        public Font SubLabelFont { get; set; } = new Font("Segoe UI", 8, FontStyle.Italic);
        public Color SubLabelForColor { get; set; } = Color.DarkSlateGray;
        public Color SubLabelBackColor { get; set; } = Color.Transparent;
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(235, 235, 235);
        public Color SubLabelHoverForeColor { get; set; } = Color.DarkSlateGray;
    }
}
