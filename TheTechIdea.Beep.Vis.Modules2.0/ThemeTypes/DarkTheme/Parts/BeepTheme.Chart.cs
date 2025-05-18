using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Chart Fonts & Colors
        public Font ChartTitleFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);
        public Font ChartSubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color ChartBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color ChartLineColor { get; set; } = Color.Cyan;
        public Color ChartFillColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color ChartAxisColor { get; set; } = Color.LightGray;
        public Color ChartTitleColor { get; set; } = Color.White;
        public Color ChartTextColor { get; set; } = Color.LightGray;
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(25, 25, 25);
        public Color ChartLegendTextColor { get; set; } = Color.White;
        public Color ChartLegendShapeColor { get; set; } = Color.Cyan;
        public Color ChartGridLineColor { get; set; } = Color.DimGray;

        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>()
        {
            Color.Cyan,
            Color.Magenta,
            Color.Lime,
            Color.Yellow,
            Color.OrangeRed,
            Color.DeepPink,
            Color.DodgerBlue
        };
    }
}
