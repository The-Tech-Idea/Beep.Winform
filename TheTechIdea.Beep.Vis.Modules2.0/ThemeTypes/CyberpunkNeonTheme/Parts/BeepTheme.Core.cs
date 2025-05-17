using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Core UI Elements

        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;

        public Color BackColor { get; set; } = Color.FromArgb(18, 18, 32);                // Deep Cyberpunk Black
        public Color PanelBackColor { get; set; } = Color.FromArgb(34, 34, 68);           // Cyberpunk Panel

        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(255, 0, 255); // Neon Magenta
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(0, 255, 255);   // Neon Cyan
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(0, 102, 255);// Neon Blue
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Horizontal;

        public Color DisabledBackColor { get; set; } = Color.FromArgb(60, 60, 80);        // Dimmed cyber black
        public Color DisabledForeColor { get; set; } = Color.FromArgb(128, 128, 160);     // Muted neon blue
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(80, 80, 128);     // Muted magenta/blue

        public Color BorderColor { get; set; } = Color.FromArgb(0, 255, 255);             // Neon Cyan
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(255, 0, 255);       // Neon Magenta
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(54, 162, 235);    // Neon Soft Blue
    }
}
