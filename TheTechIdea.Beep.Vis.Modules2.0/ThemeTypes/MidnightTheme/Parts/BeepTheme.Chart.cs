using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Chart Fonts & Colors
        public Font ChartTitleFont { get; set; } = new Font("Segoe UI", 16f, FontStyle.Bold);
        public Font ChartSubTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Italic);
        public Color ChartBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color ChartLineColor { get; set; } = Color.LightGray;
        public Color ChartFillColor { get; set; } = Color.FromArgb(70, 130, 180); // SteelBlue
        public Color ChartAxisColor { get; set; } = Color.Gray;
        public Color ChartTitleColor { get; set; } = Color.White;
        public Color ChartTextColor { get; set; } = Color.WhiteSmoke;
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ChartLegendTextColor { get; set; } = Color.White;
        public Color ChartLegendShapeColor { get; set; } = Color.SteelBlue;
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(80, 80, 80);
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(0, 188, 212), // Cyan
            Color.FromArgb(255, 193, 7), // Amber
            Color.FromArgb(233, 30, 99), // Pink
            Color.FromArgb(76, 175, 80), // Green
            Color.FromArgb(156, 39, 176), // Purple
            Color.FromArgb(255, 87, 34) // Deep Orange
        };
    }
}
