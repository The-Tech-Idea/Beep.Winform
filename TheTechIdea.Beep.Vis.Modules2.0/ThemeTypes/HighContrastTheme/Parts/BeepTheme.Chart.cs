using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Chart Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  ChartTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  ChartSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
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
    }
}
