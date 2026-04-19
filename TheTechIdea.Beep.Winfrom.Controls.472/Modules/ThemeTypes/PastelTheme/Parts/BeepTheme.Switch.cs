using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = new TypographyStyle() { FontSize = 14f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle SwitchSelectedFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle SwitchUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color SwitchBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color SwitchForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
    }
}
