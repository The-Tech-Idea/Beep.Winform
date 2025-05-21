using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(32, 32, 32);
        public Color PanelBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(32, 32, 32);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(48, 48, 48);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color DisabledForeColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color BorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(96, 96, 96);
    }
}