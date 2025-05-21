using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;

        public Color BackColor { get; set; } = Color.FromArgb(18, 18, 18);             // Very dark background
        public Color PanelBackColor { get; set; } = Color.FromArgb(30, 30, 30);        // Dark panel background
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(45, 45, 45);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(20, 20, 20);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(30, 30, 30);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;

        public Color DisabledBackColor { get; set; } = Color.FromArgb(70, 70, 70);      // Grayish disabled background
        public Color DisabledForeColor { get; set; } = Color.Gray;                     // Gray disabled text
        public Color DisabledBorderColor { get; set; } = Color.DimGray;

        public Color BorderColor { get; set; } = Color.FromArgb(70, 70, 70);           // Subtle border color
        public Color ActiveBorderColor { get; set; } = Color.Cyan;                     // Highlighted active border
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(60, 60, 60);   // Less prominent inactive border
    }
}
