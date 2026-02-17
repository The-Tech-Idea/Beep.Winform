using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Chart Fonts & Colors

        public TypographyStyle ChartTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 14f, FontStyle.Bold);
        public TypographyStyle ChartSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);

        public Color ChartBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public Color ChartLineColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink for main line
        public Color ChartFillColor { get; set; } = Color.FromArgb(210, 235, 255); // Pastel Blue area fill
        public Color ChartAxisColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint for axes
        public Color ChartTitleColor { get; set; } = Color.FromArgb(44, 62, 80); // Navy for good contrast
        public Color ChartTextColor { get; set; } = Color.FromArgb(85, 85, 85); // Soft gray for axis labels

        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(228, 222, 255); // Pastel Lavender
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(44, 62, 80); // Navy
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(255, 182, 193); // Candy Pink (shape/swatch in legend)
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(206, 183, 255); // Pastel Lavender

        // Series palette: candy pink, mint, lemon, blue, lavender, peach, aqua
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(240, 100, 180), // Candy Pink
            Color.FromArgb(127, 255, 212), // Mint
            Color.FromArgb(255, 223, 93),  // Lemon Yellow
            Color.FromArgb(54, 162, 235),  // Pastel Blue
            Color.FromArgb(228, 222, 255), // Lavender
            Color.FromArgb(255, 183, 178), // Peach
            Color.FromArgb(102, 217, 239)  // Aqua
        };
    }
}
