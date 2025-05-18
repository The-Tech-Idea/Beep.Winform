using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.Black;
        public Color RadioButtonForeColor { get; set; } = Color.White;
        public Color RadioButtonBorderColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBackColor { get; set; } = Color.Yellow;
        public Color RadioButtonCheckedForeColor { get; set; } = Color.Black;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.White;
        public Color RadioButtonHoverBackColor { get; set; } = Color.DarkGray;
        public Color RadioButtonHoverForeColor { get; set; } = Color.White;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.Yellow;
        public Font RadioButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font RadioButtonCheckedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color RadioButtonSelectedForeColor { get; set; } = Color.Black;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.White;
    }
}
