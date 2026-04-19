using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public TypographyStyle ChartSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
        public Color ChartBackColor { get; set; } = Color.Black;
        public Color ChartLineColor { get; set; } = Color.WhiteSmoke;
        public Color ChartFillColor { get; set; } = Color.Gray;
        public Color ChartAxisColor { get; set; } = Color.Silver;
        public Color ChartTitleColor { get; set; } = Color.White;
        public Color ChartTextColor { get; set; } = Color.LightGray;
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color ChartLegendTextColor { get; set; } = Color.WhiteSmoke;
        public Color ChartLegendShapeColor { get; set; } = Color.Silver;
        public Color ChartGridLineColor { get; set; } = Color.DimGray;

        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.WhiteSmoke,
            Color.Gray,
            Color.Silver,
            Color.DarkGray,
            Color.LightGray
        };
    }
}
