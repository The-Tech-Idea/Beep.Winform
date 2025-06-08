using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color ForeColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color BackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color PanelBackColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(45, 85, 120);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(36, 73, 106);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color DisabledForeColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(180, 180, 180);
        public Color BorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(200, 220, 240);
    }
}