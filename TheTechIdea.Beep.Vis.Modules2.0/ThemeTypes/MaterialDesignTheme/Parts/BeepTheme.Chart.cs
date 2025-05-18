using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Chart Fonts & Colors
        public Font ChartTitleFont { get; set; } = new Font("Roboto", 18f, FontStyle.Bold);
        public Font ChartSubTitleFont { get; set; } = new Font("Roboto", 14f, FontStyle.Regular);
        public Color ChartBackColor { get; set; } = Color.White;
        public Color ChartLineColor { get; set; } = Color.FromArgb(33, 150, 243); // Material Blue 500
        public Color ChartFillColor { get; set; } = Color.FromArgb(187, 222, 251); // Material Blue 200
        public Color ChartAxisColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Color ChartTitleColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color ChartTextColor { get; set; } = Color.FromArgb(66, 66, 66); // Grey 800
        public Color ChartLegendBackColor { get; set; } = Color.White;
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300

        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(33, 150, 243),  // Blue 500
            Color.FromArgb(244, 67, 54),   // Red 500
            Color.FromArgb(76, 175, 80),   // Green 500
            Color.FromArgb(255, 193, 7),   // Amber 500
            Color.FromArgb(156, 39, 176),  // Purple 500
            Color.FromArgb(0, 188, 212),   // Cyan 500
            Color.FromArgb(255, 87, 34),   // Deep Orange 500
            Color.FromArgb(63, 81, 181)    // Indigo 500
        };
    }
}
