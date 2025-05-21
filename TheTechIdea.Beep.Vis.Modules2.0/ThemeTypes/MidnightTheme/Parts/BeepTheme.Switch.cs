using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Switch control Fonts & Colors
//<<<<<<< HEAD
        public Font SwitchTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);
        public Font SwitchSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font SwitchUnSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);
        public Color SwitchBackColor { get; set; } = Color.FromArgb(40, 40, 50);
        public Color SwitchBorderColor { get; set; } = Color.DimGray;
        public Color SwitchForeColor { get; set; } = Color.LightGray;
        public Color SwitchSelectedBackColor { get; set; } = Color.SteelBlue;
        public Color SwitchSelectedBorderColor { get; set; } = Color.CornflowerBlue;
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(60, 60, 80);
        public Color SwitchHoverBorderColor { get; set; } = Color.LightSteelBlue;
        public Color SwitchHoverForeColor { get; set; } = Color.White;
    }
}
