using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Chart Fonts & Colors
//<<<<<<< HEAD
        public Font ChartTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Font ChartSubTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);

        public Color ChartBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color ChartLineColor { get; set; } = Color.FromArgb(70, 130, 180); // SteelBlue
        public Color ChartFillColor { get; set; } = Color.FromArgb(180, 200, 230);
        public Color ChartAxisColor { get; set; } = Color.DarkGray;
        public Color ChartTitleColor { get; set; } = Color.Black;
        public Color ChartTextColor { get; set; } = Color.Black;

        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color ChartLegendTextColor { get; set; } = Color.Black;
        public Color ChartLegendShapeColor { get; set; } = Color.SteelBlue;
        public Color ChartGridLineColor { get; set; } = Color.LightGray;

        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(102, 204, 255),   // Light Blue
            Color.FromArgb(153, 102, 255),   // Light Purple
            Color.FromArgb(255, 159, 64),    // Orange
            Color.FromArgb(75, 192, 192),    // Teal
            Color.FromArgb(255, 99, 132),    // Pink
            Color.FromArgb(54, 162, 235),    // Blue
            Color.FromArgb(255, 206, 86),    // Yellow
            Color.FromArgb(201, 203, 207)    // Gray
        };
    }
}
