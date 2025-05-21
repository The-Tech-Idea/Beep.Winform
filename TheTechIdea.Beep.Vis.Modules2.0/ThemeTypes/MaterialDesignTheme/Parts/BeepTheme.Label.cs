using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Label Colors and Fonts
//<<<<<<< HEAD
        public Color LabelBackColor { get; set; } = Color.Transparent;
        public Color LabelForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900
        public Color LabelBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Grey 400
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(66, 133, 244); // Blue 600
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(232, 240, 254); // Light Blue 50
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(227, 242, 253); // Blue 50
        public Color LabelSelectedForeColor { get; set; } = Color.FromArgb(13, 71, 161); // Blue 900
        public Color LabelDisabledBackColor { get; set; } = Color.Transparent;
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(189, 189, 189); // Grey 400
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300
        public Font LabelFont { get; set; } = new Font("Roboto", 11f, FontStyle.Regular);
        public Font SubLabelFont { get; set; } = new Font("Roboto", 10f, FontStyle.Italic);
        public Color SubLabelForColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Color SubLabelBackColor { get; set; } = Color.Transparent;
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(232, 240, 254); // Light Blue 50
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900
    }
}
