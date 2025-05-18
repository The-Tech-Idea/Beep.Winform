using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(34, 49, 34); // dark forest green
        public Color RadioButtonForeColor { get; set; } = Color.White;
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(46, 71, 46);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(56, 142, 60); // checked green
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(46, 71, 46);
        public Color RadioButtonHoverForeColor { get; set; } = Color.White;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Font RadioButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font RadioButtonCheckedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(56, 142, 60);
    }
}
