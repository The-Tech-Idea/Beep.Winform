using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle ChartSubTitleFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public Color ChartBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color ChartLineColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color ChartFillColor { get; set; } = Color.FromArgb(100, 0, 150, 200);
        public Color ChartAxisColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color ChartTitleColor { get; set; } = Color.White;
        public Color ChartTextColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color ChartLegendTextColor { get; set; } = Color.White;
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(100, 200, 255, 255);
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(0, 150, 200),
            Color.FromArgb(0, 180, 230),
            Color.FromArgb(0, 200, 240),
            Color.FromArgb(0, 130, 180),
            Color.FromArgb(0, 160, 210)
        };
    }
}