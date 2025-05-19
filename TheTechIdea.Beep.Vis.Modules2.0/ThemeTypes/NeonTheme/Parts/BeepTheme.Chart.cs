using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 16f, FontStyle.Bold);
        public TypographyStyle ChartSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 12f, FontStyle.Regular);
        public Color ChartBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for depth
        public Color ChartLineColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for lines
        public Color ChartFillColor { get; set; } = Color.FromArgb(80, 46, 204, 113); // Semi-transparent neon green for fills
        public Color ChartAxisColor { get; set; } = Color.FromArgb(100, 100, 120); // Muted gray-blue for axes
        public Color ChartTitleColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for title
        public Color ChartTextColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for general text
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for legend
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for legend text
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple for legend shapes
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(60, 60, 80); // Subtle gray for grid lines
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(26, 188, 156), // Neon turquoise
            Color.FromArgb(46, 204, 113), // Neon green
            Color.FromArgb(155, 89, 182), // Neon purple
            Color.FromArgb(241, 196, 15), // Neon yellow
            Color.FromArgb(231, 76, 60)   // Neon red
        };
    }
}