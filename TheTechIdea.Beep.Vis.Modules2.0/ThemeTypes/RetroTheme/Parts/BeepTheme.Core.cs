using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Core UI Elements
        // Note: Ensure 'Courier New' font family is available for TypographyStyle properties in other classes. If unavailable, 'Consolas' is a fallback.
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString(); // Unique identifier for the theme
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for main background
        public Color PanelBackColor { get; set; } = Color.FromArgb(0, 64, 64); // Mid-tone teal for panels
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(0, 43, 43); // Darker teal
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(0, 64, 64); // Mid-tone teal
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for retro CRT effect
        public Color DisabledBackColor { get; set; } = Color.FromArgb(64, 64, 64); // Dark gray for disabled elements
        public Color DisabledForeColor { get; set; } = Color.FromArgb(128, 128, 128); // Muted gray for disabled text
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(96, 96, 96); // Subtle gray for disabled borders
        public Color BorderColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for borders
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for active borders
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(192, 192, 192); // Light gray for inactive borders
    }
}