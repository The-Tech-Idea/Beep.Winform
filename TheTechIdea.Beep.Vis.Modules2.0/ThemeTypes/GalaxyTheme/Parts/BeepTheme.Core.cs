using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;

        public Color BackColor { get; set; } = Color.FromArgb(0x05, 0x05, 0x14); // BackgroundColor
        public Color PanelBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(0x1A, 0x1A, 0x2E); // PrimaryColor
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(0x16, 0x21, 0x3E); // SecondaryColor
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;

        public Color DisabledBackColor { get; set; } = Color.FromArgb(0x20, 0x20, 0x30); // Dark muted
        public Color DisabledForeColor { get; set; } = Color.FromArgb(0x80, 0x80, 0x80); // Gray
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Dark gray border

        public Color BorderColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color ActiveBorderColor { get; set; } = Color.White;
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(0x55, 0x55, 0x55); // Dimmed
    }
}
