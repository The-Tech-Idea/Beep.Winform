using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle SwitchSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle SwitchUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color SwitchBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color SwitchForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color SwitchHoverForeColor { get; set; } = Color.White;
    }
}