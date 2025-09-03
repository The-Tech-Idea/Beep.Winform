using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        
        public string ThemeName => this.GetType().Name;
        public Color ForeColor { get; set; } = Color.White; // Light text on dark background
        public Color BackColor { get; set; } = Color.Black;
        public Color PanelBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(20, 20, 20);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(40, 40, 40);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.Gray;
        public Color DisabledForeColor { get; set; } = Color.DarkGray;
        public Color DisabledBorderColor { get; set; } = Color.DimGray;

        public Color BorderColor { get; set; } = Color.Gray;
        public Color ActiveBorderColor { get; set; } = Color.White;
        public Color InactiveBorderColor { get; set; } = Color.DarkGray;
    }
}
