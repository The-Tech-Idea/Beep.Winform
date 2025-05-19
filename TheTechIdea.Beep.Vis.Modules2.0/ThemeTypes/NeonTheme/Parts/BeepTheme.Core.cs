using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Core UI Elements
        // Note: Ensure 'Roboto' font family is available for TypographyStyle properties in other classes. If unavailable, 'Arial' is a fallback.
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString(); // Unique identifier for the theme
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for main background
        public Color PanelBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for panels
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for sleek flow
        public Color DisabledBackColor { get; set; } = Color.FromArgb(60, 60, 80); // Muted gray for disabled elements
        public Color DisabledForeColor { get; set; } = Color.FromArgb(100, 100, 120); // Muted gray-blue for disabled text
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(80, 80, 100); // Subtle gray for disabled borders
        public Color BorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for borders
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for active borders
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(100, 100, 120); // Muted gray-blue for inactive borders
    }
}