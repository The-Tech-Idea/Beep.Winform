using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color ForeColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color BackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color PanelBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(154, 211, 240);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color DisabledForeColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(180, 180, 180);
        public Color BorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
    }
}