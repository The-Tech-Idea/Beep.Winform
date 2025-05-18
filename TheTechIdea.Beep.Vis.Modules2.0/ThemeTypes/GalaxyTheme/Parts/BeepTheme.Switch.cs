using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Switch control Fonts & Colors
        public Font SwitchTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font SwitchSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font SwitchUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color SwitchBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Subtle border
        public Color SwitchForeColor { get; set; } = Color.White;

        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(0x23, 0xB9, 0x5C); // SuccessColor
        public Color SwitchSelectedBorderColor { get; set; } = Color.White;
        public Color SwitchSelectedForeColor { get; set; } = Color.White;

        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Hover
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Highlight
        public Color SwitchHoverForeColor { get; set; } = Color.White;
    }
}
