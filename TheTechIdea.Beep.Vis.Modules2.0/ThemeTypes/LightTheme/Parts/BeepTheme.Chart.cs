using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle  ChartTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16F, FontStyle.Bold);
        public TypographyStyle  ChartSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
        public Color ChartBackColor { get; set; } = Color.White;
        public Color ChartLineColor { get; set; } = Color.DimGray;
        public Color ChartFillColor { get; set; } = Color.LightSteelBlue;
        public Color ChartAxisColor { get; set; } = Color.Gray;
        public Color ChartTitleColor { get; set; } = Color.Black;
        public Color ChartTextColor { get; set; } = Color.Black;
        public Color ChartLegendBackColor { get; set; } = Color.WhiteSmoke;
        public Color ChartLegendTextColor { get; set; } = Color.Black;
        public Color ChartLegendShapeColor { get; set; } = Color.SteelBlue;
        public Color ChartGridLineColor { get; set; } = Color.LightGray;
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.SteelBlue,
            Color.Orange,
            Color.MediumSeaGreen,
            Color.Goldenrod,
            Color.MediumPurple,
            Color.Tomato,
            Color.CadetBlue,
            Color.SlateGray
        };
    }
}
