using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Core UI Elements
        // Note: Ensure 'Roboto' font family is available for TypographyStyle properties in other classes. If unavailable, 'Arial' is a fallback.
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString(); // Unique identifier for the theme
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for main background
        public Color PanelBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for panels
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for ocean depth effect
        public Color DisabledBackColor { get; set; } = Color.FromArgb(50, 80, 110); // Lighter ocean blue for disabled elements
        public Color DisabledForeColor { get; set; } = Color.FromArgb(120, 150, 180); // Muted blue for disabled text
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(80, 110, 140); // Subtle blue for disabled borders
        public Color BorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for borders
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for active borders
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(80, 110, 140); // Muted blue for inactive borders
    }
}