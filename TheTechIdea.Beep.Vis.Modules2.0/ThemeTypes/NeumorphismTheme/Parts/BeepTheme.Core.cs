using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Core UI Elements
        // Note: Ensure 'Roboto' font family is available for TypographyStyle properties in other classes. If unavailable, 'Arial' is a fallback.
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString(); // Unique identifier for the theme
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for main background
        public Color PanelBackColor { get; set; } = Color.FromArgb(220, 220, 225); // Slightly darker gray for panels
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(240, 240, 245); // Light gray gradient start
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(210, 210, 215); // Darker gray gradient end
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(230, 230, 235); // Mid-tone gray for gradient
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for soft neumorphic effect
        public Color DisabledBackColor { get; set; } = Color.FromArgb(200, 200, 205); // Muted gray for disabled elements
        public Color DisabledForeColor { get; set; } = Color.FromArgb(150, 150, 160); // Light gray for disabled text
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(180, 180, 185); // Subtle gray for disabled borders
        public Color BorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for borders
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for active borders
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(180, 180, 185); // Muted gray for inactive borders
    }
}