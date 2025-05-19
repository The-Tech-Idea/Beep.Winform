using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Chart Fonts & Colors
<<<<<<< HEAD
        public Font ChartTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font ChartSubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color ChartBackColor { get; set; } = Color.Black;
        public Color ChartLineColor { get; set; } = Color.White;
        public Color ChartFillColor { get; set; } = Color.Gray;
        public Color ChartAxisColor { get; set; } = Color.White;
        public Color ChartTitleColor { get; set; } = Color.Yellow;
        public Color ChartTextColor { get; set; } = Color.White;
        public Color ChartLegendBackColor { get; set; } = Color.Black;
        public Color ChartLegendTextColor { get; set; } = Color.White;
        public Color ChartLegendShapeColor { get; set; } = Color.Yellow;
        public Color ChartGridLineColor { get; set; } = Color.DimGray;
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.Yellow,
            Color.Lime,
            Color.Cyan,
            Color.Magenta,
            Color.Orange,
            Color.Red
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
