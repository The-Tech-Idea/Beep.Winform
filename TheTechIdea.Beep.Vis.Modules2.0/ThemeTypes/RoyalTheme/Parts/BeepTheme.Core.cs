using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color PanelBackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(45, 45, 128);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.FromArgb(200, 200, 200); // Light gray
        public Color DisabledForeColor { get; set; } = Color.FromArgb(150, 150, 150); // Medium gray
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(180, 180, 180); // Darker gray
        public Color BorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
    }
}