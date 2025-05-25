using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color PanelBackColor { get; set; } = Color.FromArgb(240, 235, 215);
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(216, 194, 181);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.FromArgb(200, 180, 160);
        public Color DisabledForeColor { get; set; } = Color.FromArgb(150, 120, 100);
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(180, 160, 140);
        public Color BorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(200, 180, 160);
    }
}