using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
//<<<<<<< HEAD
        // Switch control Fonts & Colors with Material Design defaults
        public Font SwitchTitleFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);
        public Font SwitchSelectedFont { get; set; } = new Font("Roboto", 12f, FontStyle.Bold);
        public Font SwitchUnSelectedFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);

        public Color SwitchBackColor { get; set; } = Color.FromArgb(238, 238, 238); // Grey 200
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Grey 400
        public Color SwitchForeColor { get; set; } = Color.FromArgb(66, 66, 66); // Grey 800

        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color SwitchSelectedForeColor { get; set; } = Color.White;

        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(227, 242, 253); // Light Blue 50
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(30, 136, 229); // Blue 600
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(21, 101, 192); // Blue 700
    }
}
