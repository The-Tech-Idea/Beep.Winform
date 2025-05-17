using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        public CandyTheme()
        {
            ThemeGuid = Guid.NewGuid().ToString();
        }

        // Core UI Elements

        public string ThemeGuid { get; set; }
        public string ThemeName => this.GetType().Name;

        // Main background: soft lemon
        public Color BackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow

        // Panel background: gentle pastel pink
        public Color PanelBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink

        // Panel gradients: pink → mint → yellow
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(204, 255, 240);   // Mint
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;

        // Disabled states: muted mint/gray
        public Color DisabledBackColor { get; set; } = Color.FromArgb(232, 232, 232); // Light Gray Mint
        public Color DisabledForeColor { get; set; } = Color.FromArgb(180, 180, 180); // Muted Gray
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(210, 210, 210); // Subtle border

        // Borders
        public Color BorderColor { get; set; } = Color.FromArgb(206, 183, 255); // Pastel Lavender (soft, non-distracting)
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink (highlight/active)
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint (inactive, gentle)
    }
}
