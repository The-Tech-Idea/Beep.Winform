using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(22, 26, 30);
        public Color PanelBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(44, 48, 52);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color DisabledForeColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(66, 72, 78);
        public Color BorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(66, 72, 78);
    }
}