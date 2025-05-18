using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Switch control Fonts & Colors
        public Font SwitchTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Font SwitchSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font SwitchUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color SwitchBackColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
        public Color SwitchBorderColor { get; set; } = Color.DarkGreen;
        public Color SwitchForeColor { get; set; } = Color.White;
        public Color SwitchSelectedBackColor { get; set; } = Color.SeaGreen;
        public Color SwitchSelectedBorderColor { get; set; } = Color.LimeGreen;
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.MediumSeaGreen;
        public Color SwitchHoverBorderColor { get; set; } = Color.LightGreen;
        public Color SwitchHoverForeColor { get; set; } = Color.WhiteSmoke;
    }
}
