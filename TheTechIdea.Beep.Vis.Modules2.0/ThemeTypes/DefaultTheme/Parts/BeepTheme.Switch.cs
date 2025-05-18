using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Switch control Fonts & Colors
        public Font SwitchTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);
        public Font SwitchSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font SwitchUnSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);
        public Color SwitchBackColor { get; set; } = Color.WhiteSmoke;
        public Color SwitchBorderColor { get; set; } = Color.Gray;
        public Color SwitchForeColor { get; set; } = Color.DimGray;
        public Color SwitchSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color SwitchSelectedBorderColor { get; set; } = Color.RoyalBlue;
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.LightGray;
        public Color SwitchHoverBorderColor { get; set; } = Color.Gray;
        public Color SwitchHoverForeColor { get; set; } = Color.Black;
    }
}
