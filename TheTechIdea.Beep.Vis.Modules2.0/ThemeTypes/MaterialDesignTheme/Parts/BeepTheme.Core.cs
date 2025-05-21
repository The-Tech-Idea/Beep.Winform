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

        public Color BackColor { get; set; } = Color.FromArgb(250, 250, 250); // Very light gray background
        public Color PanelBackColor { get; set; } = Color.White; // White panels

        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(240, 240, 240);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;

        public Color DisabledBackColor { get; set; } = Color.FromArgb(224, 224, 224);
        public Color DisabledForeColor { get; set; } = Color.FromArgb(158, 158, 158);
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(189, 189, 189);

        public Color BorderColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Material Blue 500
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(224, 224, 224);
    }
}
