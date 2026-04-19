using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color ForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color BackColor { get; set; } = Color.FromArgb(250, 250, 250); // Very light gray background
        public Color PanelBackColor { get; set; } = Color.FromArgb(245, 245, 245); // White panels

        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(235, 235, 235);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(245, 245, 245);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;

        public Color DisabledBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color DisabledForeColor { get; set; } = Color.Gray;
        public Color DisabledBorderColor { get; set; } = Color.LightGray;

        public Color BorderColor { get; set; } = Color.FromArgb(224, 224, 224);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Material Blue 500
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
    }
}
