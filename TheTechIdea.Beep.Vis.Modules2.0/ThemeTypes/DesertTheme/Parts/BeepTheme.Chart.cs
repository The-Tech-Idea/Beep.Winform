using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Chart Fonts & Colors - Desert Inspired
        public Font ChartTitleFont { get; set; } = new Font("Segoe UI Semibold", 18, FontStyle.Bold);
        public Font ChartSubTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Regular);

        public Color ChartBackColor { get; set; } = Color.FromArgb(255, 250, 240); // Soft sand background
        public Color ChartLineColor { get; set; } = Color.FromArgb(150, 75, 0); // Saddle brown for lines
        public Color ChartFillColor { get; set; } = Color.FromArgb(241, 208, 160); // Clay fill color
        public Color ChartAxisColor { get; set; } = Color.FromArgb(133, 94, 66); // Dark tan for axis
        public Color ChartTitleColor { get; set; } = Color.FromArgb(111, 78, 55); // Deep brown for titles
        public Color ChartTextColor { get; set; } = Color.FromArgb(92, 64, 51); // Text color
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(255, 244, 214); // Light sand for legend background
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(133, 94, 66); // Legend text
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(180, 132, 85); // Legend shape color
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(225, 199, 160); // Subtle grid lines

        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(201, 144, 66), // Warm orange
            Color.FromArgb(222, 190, 136), // Soft sand
            Color.FromArgb(160, 82, 45),   // Sienna brown
            Color.FromArgb(210, 180, 140), // Tan
            Color.FromArgb(244, 164, 96),  // Sandy brown
            Color.FromArgb(205, 133, 63)   // Peru
        };
    }
}
