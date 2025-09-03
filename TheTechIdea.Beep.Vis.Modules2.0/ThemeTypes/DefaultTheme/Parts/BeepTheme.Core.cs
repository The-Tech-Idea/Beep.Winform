using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        // DefaultTheme: use a dark ForeColor by default so text is visible on light backgrounds
        public Color ForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color BackColor { get; set; } = Color.White;
        public Color PanelBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray panel background
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(240, 240, 240);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;

        public Color DisabledBackColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color DisabledForeColor { get; set; } = Color.Gray;
        public Color DisabledBorderColor { get; set; } = Color.LightGray;

        public Color BorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Accent Blue
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(180, 180, 180);
    }
}
