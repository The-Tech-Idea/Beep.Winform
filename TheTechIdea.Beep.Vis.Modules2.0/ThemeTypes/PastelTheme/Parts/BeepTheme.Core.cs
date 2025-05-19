using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Core UI Elements
        // Note: Ensure 'Roboto' font family is available for TypographyStyle properties in other classes. If unavailable, 'Arial' is a fallback.
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString(); // Unique identifier for the theme
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for main background
        public Color PanelBackColor { get; set; } = Color.FromArgb(255, 255, 255); // White for panels
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(220, 215, 230); // Pastel lavender
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for soft pastel effect
        public Color DisabledBackColor { get; set; } = Color.FromArgb(220, 220, 220); // Muted gray for disabled elements
        public Color DisabledForeColor { get; set; } = Color.FromArgb(150, 150, 150); // Light gray for disabled text
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(200, 200, 200); // Subtle gray for disabled borders
        public Color BorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for borders
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for active borders
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(200, 200, 200); // Muted gray for inactive borders
    }
}