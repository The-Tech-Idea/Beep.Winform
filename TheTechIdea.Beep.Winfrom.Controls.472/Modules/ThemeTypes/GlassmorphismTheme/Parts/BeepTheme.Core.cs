using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; }
        public string ThemeName => this.GetType().Name;
        public Color ForeColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color BackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color PanelBackColor { get; set; } = Color.FromArgb(240, 245, 250);

        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(215, 230, 245);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;

        public Color DisabledBackColor { get; set; } = Color.FromArgb(220, 220, 220);
        public Color DisabledForeColor { get; set; } = Color.Gray;
        public Color DisabledBorderColor { get; set; } = Color.DarkGray;

        public Color BorderColor { get; set; } = Color.FromArgb(180, 200, 220);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(100, 180, 255);
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(200, 210, 220);
    }
}
