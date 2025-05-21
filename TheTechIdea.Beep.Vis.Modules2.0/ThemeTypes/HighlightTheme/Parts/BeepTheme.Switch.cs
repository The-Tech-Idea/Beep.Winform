using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Switch control Fonts & Colors
//<<<<<<< HEAD
        public Font SwitchTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font SwitchSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font SwitchUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color SwitchBackColor { get; set; } = Color.LightSteelBlue;
        public Color SwitchBorderColor { get; set; } = Color.SteelBlue;
        public Color SwitchForeColor { get; set; } = Color.Black;
        public Color SwitchSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color SwitchSelectedBorderColor { get; set; } = Color.MediumBlue;
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.RoyalBlue;
        public Color SwitchHoverBorderColor { get; set; } = Color.RoyalBlue;
        public Color SwitchHoverForeColor { get; set; } = Color.White;
    }
}
