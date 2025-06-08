using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color ForeColor { get; set; } = Color.White;
        public Color BackColor { get; set; } = Color.White;
        public Color PanelBackColor { get; set; } = Color.FromArgb(240, 243, 250);
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(225, 230, 245);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(240, 243, 250);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.LightGray;
        public Color DisabledForeColor { get; set; } = Color.Gray;
        public Color DisabledBorderColor { get; set; } = Color.DarkGray;

        public Color BorderColor { get; set; } = Color.LightSteelBlue;
        public Color ActiveBorderColor { get; set; } = Color.DodgerBlue;
        public Color InactiveBorderColor { get; set; } = Color.LightGray;
    }
}
