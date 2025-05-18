using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Chart Fonts & Colors
        public Font ChartTitleFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);
        public Font ChartSubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color ChartBackColor { get; set; } = Color.White;
        public Color ChartLineColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color ChartFillColor { get; set; } = Color.FromArgb(100, 181, 246); // Blue 300 translucent
        public Color ChartAxisColor { get; set; } = Color.FromArgb(158, 158, 158); // Grey 500
        public Color ChartTitleColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark grey
        public Color ChartTextColor { get; set; } = Color.FromArgb(66, 66, 66); // Medium grey
        public Color ChartLegendBackColor { get; set; } = Color.WhiteSmoke;
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(66, 66, 66);
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(33, 150, 243);
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(224, 224, 224); // Light grey grid
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(33, 150, 243),   // Blue 500
            Color.FromArgb(244, 67, 54),    // Red 500
            Color.FromArgb(76, 175, 80),    // Green 500
            Color.FromArgb(255, 193, 7),    // Amber 500
            Color.FromArgb(156, 39, 176),   // Purple 500
            Color.FromArgb(255, 87, 34),    // Deep Orange 500
        };
    }
}
