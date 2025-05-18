using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Chart Fonts & Colors
        public Font ChartTitleFont { get; set; } = new Font("Segoe UI", 18F, FontStyle.Bold);
        public Font ChartSubTitleFont { get; set; } = new Font("Segoe UI", 14F, FontStyle.Regular);
        public Color ChartBackColor { get; set; } = Color.White;
        public Color ChartLineColor { get; set; } = Color.FromArgb(50, 50, 50); // Dark Gray
        public Color ChartFillColor { get; set; } = Color.FromArgb(173, 216, 230); // Light Blue
        public Color ChartAxisColor { get; set; } = Color.FromArgb(100, 100, 100); // Medium Gray
        public Color ChartTitleColor { get; set; } = Color.FromArgb(33, 33, 33); // Almost Black
        public Color ChartTextColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(240, 240, 240); // Light Gray
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(54, 162, 235); // Soft Blue
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(230, 230, 230); // Very Light Gray

        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(54, 162, 235), // Blue
            Color.FromArgb(255, 99, 132), // Red
            Color.FromArgb(255, 206, 86), // Yellow
            Color.FromArgb(75, 192, 192), // Teal
            Color.FromArgb(153, 102, 255), // Purple
            Color.FromArgb(255, 159, 64) // Orange
        };
    }
}
