using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public TypographyStyle RadioButtonFont { get; set; }
        public TypographyStyle RadioButtonCheckedFont { get; set; }
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
    }
}
