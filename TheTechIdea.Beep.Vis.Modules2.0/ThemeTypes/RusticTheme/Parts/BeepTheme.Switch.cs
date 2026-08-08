using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; }
        public TypographyStyle SwitchSelectedFont { get; set; }
        public TypographyStyle SwitchUnSelectedFont { get; set; }
        public Color SwitchBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color SwitchForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
    }
}
