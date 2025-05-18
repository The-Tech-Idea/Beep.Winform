using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();
        public string ThemeName => this.GetType().Name;
        public Color BackColor { get; set; } = Color.FromArgb(34, 49, 34); // Dark Forest Green
        public Color PanelBackColor { get; set; } = Color.FromArgb(46, 70, 46); // Medium Dark Green
        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(56, 87, 56);
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(23, 47, 23);
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(40, 70, 40);
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.Vertical;
        public Color DisabledBackColor { get; set; } = Color.FromArgb(80, 80, 80); // Grayish disabled background
        public Color DisabledForeColor { get; set; } = Color.FromArgb(130, 130, 130); // Gray text for disabled
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(100, 100, 100); // Disabled border

        public Color BorderColor { get; set; } = Color.FromArgb(34, 60, 34); // Deep forest border
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(56, 100, 56); // Active border highlight
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(23, 40, 23); // Inactive border subtle
    }
}
