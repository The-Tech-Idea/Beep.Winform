using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16F, FontStyle.Bold);
        public TypographyStyle ChartSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
        public Color ChartBackColor { get; set; } = Color.FromArgb(240, 255, 240); // Very light green background
        public Color ChartLineColor { get; set; } = Color.ForestGreen;
        public Color ChartFillColor { get; set; } = Color.FromArgb(180, 230, 180);
        public Color ChartAxisColor { get; set; } = Color.SeaGreen;
        public Color ChartTitleColor { get; set; } = Color.DarkGreen;
        public Color ChartTextColor { get; set; } = Color.ForestGreen;
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(220, 245, 220);
        public Color ChartLegendTextColor { get; set; } = Color.DarkGreen;
        public Color ChartLegendShapeColor { get; set; } = Color.SeaGreen;
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(180, 220, 180);
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.ForestGreen,
            Color.SeaGreen,
            Color.OliveDrab,
            Color.DarkOliveGreen,
            Color.MediumSeaGreen,
            Color.LimeGreen
        };
    }
}
