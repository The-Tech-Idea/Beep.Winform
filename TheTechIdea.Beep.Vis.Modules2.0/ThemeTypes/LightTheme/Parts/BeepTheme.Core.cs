using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;

        public Color BackColor { get; set; } = Color.White;
        public Color PanelBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color PanelGradiantStartColor { get; set; } = Color.White;
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(240, 240, 240);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;

        public Color DisabledBackColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color DisabledForeColor { get; set; } = Color.Gray;
        public Color DisabledBorderColor { get; set; } = Color.LightGray;

        public Color BorderColor { get; set; } = Color.LightGray;
        public Color ActiveBorderColor { get; set; } = Color.DodgerBlue;
        public Color InactiveBorderColor { get; set; } = Color.Gray;
    }
}
