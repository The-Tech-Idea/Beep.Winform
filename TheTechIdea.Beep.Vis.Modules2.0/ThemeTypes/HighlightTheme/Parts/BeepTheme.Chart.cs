using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Chart Fonts & Colors
<<<<<<< HEAD
        public Font ChartTitleFont { get; set; } = new Font("Segoe UI", 18, FontStyle.Bold);
        public Font ChartSubTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Regular);
        public Color ChartBackColor { get; set; } = Color.White;
        public Color ChartLineColor { get; set; } = Color.FromArgb(30, 144, 255); // DodgerBlue
        public Color ChartFillColor { get; set; } = Color.FromArgb(135, 206, 250); // LightSkyBlue
        public Color ChartAxisColor { get; set; } = Color.Black;
        public Color ChartTitleColor { get; set; } = Color.Black;
        public Color ChartTextColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ChartLegendBackColor { get; set; } = Color.WhiteSmoke;
        public Color ChartLegendTextColor { get; set; } = Color.Black;
        public Color ChartLegendShapeColor { get; set; } = Color.DodgerBlue;
        public Color ChartGridLineColor { get; set; } = Color.LightGray;
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(30, 144, 255), // DodgerBlue
            Color.FromArgb(255, 165, 0),  // Orange
            Color.FromArgb(34, 139, 34),  // ForestGreen
            Color.FromArgb(220, 20, 60),  // Crimson
            Color.FromArgb(148, 0, 211),  // DarkViolet
            Color.FromArgb(255, 105, 180) // HotPink
        };
=======
        public TypographyStyle ChartTitleFont { get; set; }
        public TypographyStyle ChartSubTitleFont { get; set; }
        public Color ChartBackColor { get; set; }
        public Color ChartLineColor { get; set; }
        public Color ChartFillColor { get; set; }
        public Color ChartAxisColor { get; set; }
        public Color ChartTitleColor { get; set; }
        public Color ChartTextColor { get; set; }
        public Color ChartLegendBackColor { get; set; }
        public Color ChartLegendTextColor { get; set; }
        public Color ChartLegendShapeColor { get; set; }
        public Color ChartGridLineColor { get; set; }
        public List<Color> ChartDefaultSeriesColors { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
