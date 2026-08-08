using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
   
    public partial class RusticTheme
    {
  
        // Core UI Elements
        public string ThemeGuid { get; set; }
        public string ThemeName => this.GetType().Name;
        public Color ForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color BackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color PanelBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(210, 180, 140);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(238, 232, 205);
        public LinearGradientMode PanelGradiantDirection { get; set; }
        public Color DisabledBackColor { get; set; } = Color.FromArgb(227, 220, 203);
        public Color DisabledForeColor { get; set; } = Color.FromArgb(150, 140, 120);
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(200, 190, 170);

        public Color BorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
    }
}
