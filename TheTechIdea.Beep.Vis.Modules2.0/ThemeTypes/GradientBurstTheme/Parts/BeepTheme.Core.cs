using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Core UI Elements
        public string ThemeGuid { get; set; }
        public string ThemeName => this.GetType().Name;

        public Color BackColor { get; set; } = Color.FromArgb(245, 245, 245); // Very light gray
        public Color PanelBackColor { get; set; } = Color.White;

        public Color PanelGradiantStartColor { get; set; } = Color.FromArgb(63, 81, 181);   // Indigo
        public Color PanelGradiantEndColor { get; set; } = Color.FromArgb(255, 87, 34);   // Deep Orange
        public Color PanelGradiantMiddleColor { get; set; } = Color.FromArgb(103, 58, 183);  // Deep Purple
        public LinearGradientMode PanelGradiantDirection { get; set; } = LinearGradientMode.ForwardDiagonal;

        public Color DisabledBackColor { get; set; } = Color.FromArgb(238, 238, 238); // Gray
        public Color DisabledForeColor { get; set; } = Color.FromArgb(130, 130, 130); // Dim text
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Soft Gray

        public Color BorderColor { get; set; } = Color.FromArgb(117, 117, 117); // Gray
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(63, 81, 181);   // Indigo
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Soft Gray
    }
}
