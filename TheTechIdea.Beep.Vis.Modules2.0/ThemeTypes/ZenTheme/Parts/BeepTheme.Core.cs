using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color PanelBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(48, 48, 48);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color DisabledForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color BorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(189, 189, 189);
    }
}