using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(18, 18, 18); // Very dark background
        public Color PanelBackColor { get; set; } = Color.FromArgb(28, 28, 28); // Slightly lighter panel background
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(38, 38, 38);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(18, 18, 18);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(28, 28, 28);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color DisabledForeColor { get; set; } = Color.Gray;
        public Color DisabledBorderColor { get; set; } = Color.DimGray;

        public Color BorderColor { get; set; } = Color.DarkSlateGray;
        public Color ActiveBorderColor { get; set; } = Color.CornflowerBlue;
        public Color InactiveBorderColor { get; set; } = Color.Gray;
    }
}
