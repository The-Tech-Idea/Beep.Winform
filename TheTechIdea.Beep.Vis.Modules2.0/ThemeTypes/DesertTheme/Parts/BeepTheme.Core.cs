using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;

        public Color BackColor { get; set; } = Color.FromArgb(255, 244, 229); // Soft beige background
        public Color PanelBackColor { get; set; } = Color.FromArgb(245, 222, 179); // Wheat tone for panels
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(255, 248, 220); // Light cream
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan desert sand
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(238, 214, 175); // Pale gold

        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;

        public Color DisabledBackColor { get; set; } = Color.FromArgb(210, 180, 140, 140); // Semi-transparent tan
        public Color DisabledForeColor { get; set; } = Color.FromArgb(169, 169, 169); // Dark Gray
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(205, 192, 176); // Light taupe

        public Color BorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan border
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(160, 110, 50); // Darker brown for active border
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(222, 184, 135); // Light brown for inactive border
    }
}
