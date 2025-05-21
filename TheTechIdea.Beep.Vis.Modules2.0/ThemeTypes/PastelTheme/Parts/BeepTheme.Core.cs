using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color PanelBackColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(247, 221, 229);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(242, 201, 215);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color DisabledForeColor { get; set; } = Color.FromArgb(180, 180, 180);
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color BorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
    }
}