using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.White;
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Gray
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(158, 158, 158); // Medium Gray
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(30, 136, 229); // Blue 600
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(232, 240, 254); // Light Blue Hover
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(100, 181, 246); // Blue 300
        public Font CheckBoxFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font CheckBoxCheckedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
    }
}
