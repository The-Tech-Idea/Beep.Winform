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
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public Color PanelBackColor { get; set; }
        public Color PanelGradiantStartColor { get; set; }
        public Color PanelGradiantEndColor { get; set; }
        public Color PanelGradiantMiddleColor { get; set; }
        public LinearGradientMode PanelGradiantDirection { get; set; }
        public Color DisabledBackColor { get; set; }
        public Color DisabledForeColor { get; set; }
        public Color DisabledBorderColor { get; set; }

        public Color BorderColor { get; set; }
        public Color ActiveBorderColor { get; set; }
        public Color InactiveBorderColor { get; set; }
    }
}
