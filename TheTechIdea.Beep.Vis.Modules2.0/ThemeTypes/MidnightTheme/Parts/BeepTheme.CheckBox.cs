using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color CheckBoxForeColor { get; set; } = Color.WhiteSmoke;
        public Color CheckBoxBorderColor { get; set; } = Color.Gray;
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(0, 150, 136); // Teal 500
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(0, 137, 123); // Teal 700
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(55, 71, 79); // Blue Grey 800
        public Color CheckBoxHoverForeColor { get; set; } = Color.White;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(0, 188, 212); // Cyan 500
        public Font CheckBoxFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font CheckBoxCheckedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
    }
}
