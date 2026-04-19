using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color ForeColor { get; set; } = Color.Black;
        public Color BackColor { get; set; } = Color.Black;
        public Color PanelBackColor { get; set; } = Color.FromArgb(20, 20, 20);
        public Color PanelGradiantStartColor { get; set; } = Color.Black;
        public Color PanelGradiantEndColor { get; set; } = Color.Gray;
        public Color PanelGradiantMiddleColor { get; set; } = Color.DimGray;
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;

        public Color DisabledBackColor { get; set; } = Color.DarkGray;
        public Color DisabledForeColor { get; set; } = Color.LightGray;
        public Color DisabledBorderColor { get; set; } = Color.Gray;

        public Color BorderColor { get; set; } = Color.White;
        public Color ActiveBorderColor { get; set; } = Color.Yellow;
        public Color InactiveBorderColor { get; set; } = Color.Gray;
    }
}
