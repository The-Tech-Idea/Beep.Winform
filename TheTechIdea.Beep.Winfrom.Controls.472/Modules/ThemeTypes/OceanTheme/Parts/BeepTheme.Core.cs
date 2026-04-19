using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color ForeColor { get; set; } = Color.FromArgb(240, 245, 250);
        public Color BackColor { get; set; } = Color.FromArgb(240, 245, 250);
        public Color PanelBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(0, 105, 148);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.FromArgb(200, 210, 220);
        public Color DisabledForeColor { get; set; } = Color.FromArgb(150, 160, 170);
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(180, 190, 200);
        public Color BorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(0, 100, 150);
    }
}