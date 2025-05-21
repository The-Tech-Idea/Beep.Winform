using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Switch control Fonts & Colors
//<<<<<<< HEAD
        public Font SwitchTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Font SwitchSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font SwitchUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color SwitchBackColor { get; set; } = Color.White;
        public Color SwitchBorderColor { get; set; } = Color.LightGray;
        public Color SwitchForeColor { get; set; } = Color.Black;
        public Color SwitchSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color SwitchSelectedBorderColor { get; set; } = Color.Blue;
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.LightBlue;
        public Color SwitchHoverBorderColor { get; set; } = Color.SteelBlue;
        public Color SwitchHoverForeColor { get; set; } = Color.DarkBlue;
    }
}
