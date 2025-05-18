using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color RadioButtonForeColor { get; set; } = Color.White;
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Subtle border

        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.White;

        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Hover background
        public Color RadioButtonHoverForeColor { get; set; } = Color.White;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Highlight border

        public Font RadioButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font RadioButtonCheckedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);

        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
    }
}
