using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Switch control Fonts & Colors
        public Font SwitchTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Font SwitchSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font SwitchUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color SwitchBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color SwitchBorderColor { get; set; } = Color.DimGray;
        public Color SwitchForeColor { get; set; } = Color.LightGray;

        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(0, 122, 204);
        public Color SwitchSelectedBorderColor { get; set; } = Color.DeepSkyBlue;
        public Color SwitchSelectedForeColor { get; set; } = Color.White;

        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color SwitchHoverBorderColor { get; set; } = Color.LightBlue;
        public Color SwitchHoverForeColor { get; set; } = Color.White;
    }
}
