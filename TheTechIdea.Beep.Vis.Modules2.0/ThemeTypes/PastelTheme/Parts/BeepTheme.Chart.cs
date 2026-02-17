using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle() { FontSize = 14f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle ChartSubTitleFont { get; set; } = new TypographyStyle() { FontSize = 12f, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public Color ChartBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color ChartLineColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color ChartFillColor { get; set; } = Color.FromArgb(150, 245, 183, 203);
        public Color ChartAxisColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color ChartTitleColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ChartTextColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(200, 242, 201, 215);
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(245, 183, 203),
            Color.FromArgb(255, 204, 221),
            Color.FromArgb(237, 181, 201),
            Color.FromArgb(247, 221, 229),
            Color.FromArgb(255, 224, 239)
        };
    }
}
